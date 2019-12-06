﻿// ----------------------------------------------------------------------------------------------------------
// This file contains logic for calling the Web PKI component to sign a batch of documents optimizely. It is
// only an example, feel free to alter it to meet your application's needs.
// ----------------------------------------------------------------------------------------------------------
var batchSignatureOptimizedForm = (function () {

	// The Javascript class "Queue" defined here helps to process the documents in the batch. You don't
	// necessarily need to understand this code, only how to use it. (see the usage below on the function
	// startBatch)
	(function () {
		window.Queue = function () {
			this.items = [];
			this.writerCount = 0;
			this.readerCount = 0;
		};
		window.Queue.prototype.add = function (e) {
			this.items.push(e);
		};
		window.Queue.prototype.addRange = function (array) {
			for (var i = 0; i < array.length; i++) {
				this.add(array[i]);
			}
		};
		var _process = function (inQueue, processor, outQueue, endCallback) {
			var obj = inQueue.items.shift();
			if (obj !== undefined) {
				processor(obj, function (result) {
					if (result != null && outQueue != null) {
						outQueue.add(result);
					}
					_process(inQueue, processor, outQueue, endCallback);
				});
			} else if (inQueue.writerCount > 0) {
				setTimeout(function () {
					_process(inQueue, processor, outQueue, endCallback);
				}, 200);
			} else {
				--inQueue.readerCount;
				if (outQueue != null) {
					--outQueue.writerCount;
				}
				if (inQueue.readerCount == 0 && endCallback) {
					endCallback();
				}
			}
		};
		window.Queue.prototype.process = function (processor, options) {
			var threads = options.threads || 1;
			this.readerCount = threads;
			if (options.output) {
				options.output.writerCount = threads;
			}
			for (var i = 0; i < threads; i++) {
				_process(this, processor, options.output, options.completed);
			}
		};
	})();

	// Auxiliary global variables.
	var selectedCertThumbprint = null;
	var batchId = null;
	var startQueue = null;
	var performQueue = null;
	var completeQueue = null;
	var batchDocIds = null;
	var selectElement = null;
	var docListElement = null;
	var signButton = null;
	var refreshButton = null;
	var startTime = null;
	var endTime = null;

	// Create an instance of the LacunaWebPKI object.
	var pki = new LacunaWebPKI(_webPkiLicense);

	// ------------------------------------------------------------------------------------------------------
	// Initializes the batch signature form.
	// ------------------------------------------------------------------------------------------------------
	function init(args) {

		selectElement = args.certificateSelect;
		docListElement = args.docList;
		signButton = args.signButton;
		refreshButton = args.refreshButton;
		controllerEndpoint = args.controllerEndpoint;

		// Receive the documents ids.
		batchDocIds = args.documentsIds;

		// Wireup of button clicks.
		signButton.click(sign);
		refreshButton.click(refresh);

		// Block the UI while we get things ready.
		$.blockUI('Initializing ...');

		// Render documents to be signed.
		for (var i = 0; i < batchDocIds.length; i++) {
			var docId = batchDocIds[i];
			docListElement.append(
				$('<li />').append(
					$('<a />').text('Document ' + docId).attr('href', '/Download/Doc/' + docId)
				)
			);
		}

		// Call the init() method on the LacunaWebPKI object, passing a callback for when the component is
		// ready to be used and another to be called when an error occurrs on any of the subsequent
		// operations. For more information, see:
		// https://docs.lacunasoftware.com/en-us/articles/web-pki/get-started.html#coding-the-first-lines
		// http://webpki.lacunasoftware.com/Help/classes/LacunaWebPKI.html#method_init
		pki.init({
			ready: loadCertificates, // As soon as the component is ready we'll load the certificates.
			defaultError: onWebPkiError, // Generic error callback on Content/js/app/site.js.
			restPkiUrl: _restPkiEndpoint
		});

	}

	// ------------------------------------------------------------------------------------------------------
	// Function called when the user clicks the "Refresh" button.
	// ------------------------------------------------------------------------------------------------------
	function refresh() {
		// Block the UI while we load the certificates.
		$.blockUI();
		// Invoke the loading of the certificates.
		loadCertificates();
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that loads the certificates, either on startup or when the user clicks the "Refresh" button.
	// At this point, the UI is already blocked.
	// ------------------------------------------------------------------------------------------------------
	function loadCertificates() {

		// Call the listCertificates() method to list the user's certificates. For more information see:
		// http://webpki.lacunasoftware.com/Help/classes/LacunaWebPKI.html#method_listCertificates
		pki.listCertificates({

			// Specify that expired certificates should be ignored.
			filter: pki.filters.isWithinValidity,

			// In order to list only certificates within validity period and having a CPF (ICP-Brasil), use
			// this instead:
			//filter: pki.filters.all(pki.filters.hasPkiBrazilCpf, pki.filters.isWithinValidity),

			// ID of the select to be populated with the certificates.
			selectId: selectElement.attr('id'),

			// Function that will be called to get the text that should be displayed for each option.
			selectOptionFormatter: function (cert) {
				var s = cert.subjectName + ' (issued by ' + cert.issuerName + ')';
				if (new Date() > cert.validityEnd) {
					s = '[EXPIRED] ' + s;
				}
				return s;
			}

		}).success(function () {

			// Once the certificates have been listed, unblock the UI.
			$.unblockUI();

		});
	}

	// ------------------------------------------------------------------------------------------------------
	// Function called when the user clicks the "Sign Batch" button.
	// ------------------------------------------------------------------------------------------------------
	function sign() {

		// Block the UI while we perform the signature.
		$.blockUI({ message: 'Signing ...' });

		// Get the thumbprint of the selected certificate and store it in a global variable. (we'll need it
		// later)
		selectedCertThumbprint = selectElement.val();

		// Call Web PKI to preauthorize the signatures, so that the user only sees one confirmation dialog.
		pki.preauthorizeSignatures({
			certificateThumbprint: selectedCertThumbprint,
			signatureCount: batchDocIds.length // Number of signatures to be authorized by the user.
		}).success(initBatch); // Callback to be called if the user authorizes the signatures.
	}

	// ------------------------------------------------------------------------------------------------------
	// Function called when the user authorizes the signatures.
	// ------------------------------------------------------------------------------------------------------
	function initBatch() {
		// The first thing we need to do is read the selected certificate's content.
		pki.readCertificate(selectedCertThumbprint).success(function (certContent) {
			// Once we have the certificate content, we'll post it to the server asynchronously.
			$.ajax({
				url: controllerEndpoint + '/Init',
				method: 'POST',
				data: {
					certificate: certContent
				},
				dataType: 'json',
				success: function (response) {
					// The server will store the certificate content and return the batch ID, which we must
					// pass on subsequent calls. (so we'll store it on a global variable)
					batchId = response.batchId;
					// Start processing the batch.
					startBatch();
				},
				error: function (jqXHR, textStatus, errorThrown) {
					addAlert('danger', 'Batch could not be started because an error has occurred: ' + errorThrown || textStatus);
					$.unblockUI();
				}
			});
		});
	}

	// ------------------------------------------------------------------------------------------------------
	// Function called when the batch is successfully initialized.
	// ------------------------------------------------------------------------------------------------------
	function startBatch() {
		startTime = performance.now();

        /*
            For each document, we must perform 3 actions in sequence:

            1. Start the signature    : Call the action /BatchSignature/Start to start the signature and get
                                        the parameters for the signHash call.
            2. Perform the signature  : Call Web PKI's method signHash with the parameters acquired on the
                                        previous step.
            3. Complete the signature : Call the action /BatchSignature/Complete sending the RSA signature
                                        result.

            We'll use the Queue Javascript class defined above in order to perform these steps
            simultaneously.
         */

		// Create the queues.
		startQueue = new Queue();
		performQueue = new Queue();
		completeQueue = new Queue();

		// Add all documents to the first ("start") queue.
		for (var i = 0; i < batchDocIds.length; i++) {
			startQueue.add({ index: i, docId: batchDocIds[i] });
		}

        /*
            Process each queue placing the result on the next queue, forming a sort of "assembly line":

                startQueue                               performQueue                               completeQueue
                -------------                            -------------                              -------------
                      XXXXXXX  ->  (startSignature)  ->             XX  ->  (performSignature)  ->            XXX  ->  (completeSignature)
                -------------         3 threads          -------------          1 threads           -------------           3 threads
         */
		startQueue.process(startSignature, { threads: 3, output: performQueue });
		performQueue.process(performSignature, { threads: 1, output: completeQueue });
		completeQueue.process(completeSignature, { threads: 3, completed: onBatchCompleted });

		// Notice: the thread count on each call above is already optimized, increasing the number of threads
		// will not enhance the performance significatively.
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that performs the first step described above for each document, which is the call to the
	// action BatchSignature/Start in order to start the signature and get the parameters for the signHash
	// call(performed in the second step).
	//
	// This function is called by the Queue.process function, taking documents from the "start" queue. Once
	// we're done, we'll call the "done" callback passing the document, and the Queue.process function will
	// place the document on the "perform" queue to await processing.
	// ------------------------------------------------------------------------------------------------------
	function startSignature(step, done) {
		// Call the server asynchronously to start the signature. (the server will call REST PKI and will
		// return the signature operation token along with the parameters for the signHash call)
		$.ajax({
			url: controllerEndpoint + '/Start',
			method: 'POST',
			dataType: 'json',
			data: {
				batchId: batchId,
				documentId: step.docId
			},
			success: function (response) {
				// Add the returned information to the document information. (we'll need it in the second
				// step)
				step.toSignHash = response.toSignHash;
				step.digestAlgorithmOid = response.digestAlgorithmOid;
				step.transferDataFileId = response.transferDataFileId;
				// Call the "done" callback signalling we're done with the document.
				done(step);
			},
			error: function (jqXHR, textStatus, errorThrown) {
				// Render error.
				renderFail(step, errorThrown || textStatus);
				// Call the "done" callback with no argument, signalling the document should not go to the
				// next queue.
				done();
			}
		});
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that performs the second step described above for each document, which is the call to
	// Web PKI's signHash function using the parameters acquired on the first step.
	//
	// This function is called by the Queue.process function, taking documents from the "perform" queue. Once
	// we're done, we'll call the "done" callback passing the document, and the Queue.process function will
	// place the document on the "complete" queue to await processing.
	// ------------------------------------------------------------------------------------------------------
	function performSignature(step, done) {
		// Call signHash() on the Web PKI component passing the parameters received from REST PKI and the
		// certificate selected by the user.
		pki.signHash({
			thumbprint: selectedCertThumbprint,
			hash: step.toSignHash,
			digestAlgorithm: step.digestAlgorithmOid
		}).success(function (signature) {
			// Add the signature result to the document information. (we'll need it in the third step)
			step.signature = signature;
			// Call the "done" callback signalling we're done with the document.
			done(step);
		}).error(function (error) {
			// Render error.
			renderFail(step, error);
			// Call the "done" callback with no argument, signalling the document should not go to the next
			// queue.
			done();
		});
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that performs the third step described above for each document, which is the call to the
	// action BatchSignature/Complete in order to complete the signature.
	//
	// This function is called by the Queue.process function, taking documents from the "complete" queue.
	// Once we're done, we'll call the "done" callback passing the document.Once all documents are processed,
	// the Queue.process will call the "onBatchCompleted" function.
	// ------------------------------------------------------------------------------------------------------
	function completeSignature(step, done) {
		// Call the server asynchronously sending the signature result.
		$.ajax({
			url: controllerEndpoint + '/Complete',
			method: 'POST',
			dataType: 'json',
			data: {
				transferDataFileId: step.transferDataFileId,
				signature: step.signature
			},
			success: function (signedFileId) {
				step.signedFileId = signedFileId;
				// Render success.
				renderSuccess(step);
				// Call the "done" callback signalling we're done with the document.
				done(step);
			},
			error: function (jqXHR, textStatus, errorThrown) {
				// Render error.
				renderFail(step, errorThrown || textStatus);
				// Call the "done" callback with no argument, signalling the document should not go to the
				// next queue.
				done();
			}
		});
	}

	// ------------------------------------------------------------------------------------------------------
	// Function called once the batch is completed.
	// ------------------------------------------------------------------------------------------------------
	function onBatchCompleted() {
		endTime = performance.now();
		// Notify the user and unblock the UI.
		addAlert('primary', 'Batch processing completed in ' + ((endTime - startTime) / 1000).toFixed(2) + ' seconds.');
		addAlert('primary', 'Each document tooks approximately ' + ((endTime - startTime) / batchDocIds.length).toFixed(2) + ' milliseconds.');
		addAlert('primary', 'It signs approximately ' + ((batchDocIds.length * 1000) / (endTime - startTime)).toFixed(2) + ' documents for seconds.');

		// Prevent user from clicking "sign batch" again. (our logic isn't prepared for that)
		signButton.prop('disabled', true);
		// Unblock the UI.
		$.unblockUI();
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that renders a documument as completed successfully.
	// ------------------------------------------------------------------------------------------------------
	function renderSuccess(step) {
		var docLi = docListElement.find('li').eq(step.index);
		docLi
		.append(document.createTextNode(' '))
		.append($('<i>')
			.addClass('fas fa-arrow-right'))
		.append(document.createTextNode(' '))
		.append($('<a>')
			.text(step.signedFileId.signedFileId.replace('_', '.'))
			.attr('href', '/Download/File/' + step.signedFileId.signedFileId)
		);
	}

	// ------------------------------------------------------------------------------------------------------
	// Function that renders a documument as failed.
	// ------------------------------------------------------------------------------------------------------
	function renderFail(step, error) {
		addAlert('danger', 'An error has occurred while signing Document ' + step.docId + ': ' + error);
		var docLi = docListElement.find('li').eq(step.index);
		docLi.append(
			document.createTextNode(' ')
		).append(
			$('<span />').addClass('glyphicon glyphicon-remove')
		);
	}

	// ------------------------------------------------------------------------------------------------------
	// Function called if an error occurs on the Web PKI component.
	// ------------------------------------------------------------------------------------------------------
	function onWebPkiError(message, error, origin) {

		// Unblock the UI.
		$.unblockUI();

		// Log the error to the browser console. (for debugging purposes)
		if (console) {
			console.log('An error has occurred on the signature browser component: ' + message, error);
		}

		// Show the message to the user. You might want to substitute the alert below with a more
		// user-friendly UI component to show the error.
		addAlert('danger', 'An error has occurred on the signature browser component: ' + message);
	}

	return {
		init: init
	};

})();