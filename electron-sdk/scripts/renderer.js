const path = require('path');
const fs = require('fs');

const { remote, shell} = require('electron'),
		dotnet = remote.getGlobal('dotnet'),
		{ dialog } = remote;

/**
 * Global variables to used between pages.
 */
let globalVariables = {
	certs: null,
	file: null
};
function reset() {
	resetCerts();
	globalVariables.file = null;
	$('#chosenFile').empty();
	$('#chooseFileBtn')
		.text('Choose File')
		.removeClass('btn-dark')
		.addClass('btn-outline-dark');
}
function resetCerts() {
	globalVariables.certs = null;
	$('#listCerts #certificateSelect').empty();
	$('#signPdf #certificateSelect').empty();
}


/**
 * Application Routing using JQuery.
 */
let currentPage = 'home';
function goTo(route) {
	if (route === currentPage) return;

	// Show desired partial and hide current partial.
	$(`#${currentPage}`).addClass('ls-inactive');
	$(`#${route}`).removeClass('ls-inactive');
	currentPage = route;

	reset();
	switch (route) {
		case 'list-certs':
			listCertsInit();
			break;
		case 'sign-pdf':
			signPdfInit();
			break;
	}
}

/**
 * "List Certificates" page initialization.
 */
function listCertsInit() {

	dotnet.ping(() => {
		dotnet.sendRequest('list-certs', null, (response, err) => {
			if (err) console.log(err);

			for (let cert of response) {

				var text = '';
				if (cert.isExpired) {
					text += '[EXPIRED] ';
				}
				text += cert.subjectName.commonName;
				if (!cert.isSelfSigned) {
					text += ` (issued by ${cert.issuerName.commonName})`
				}
				$('#listCerts #certificateSelect')
					.append($('<option>', {
						value: cert.thumbprintSHA1,
						text: text
					}));
			}
			
		});
	});
}

// Register "Refresh Certificates" button.
function listCertsRefreshCerts() {
	resetCerts();
	listCertsInit();
};

/**
 * "Sign a PDF" page initialization.
 */
function signPdfInit() {

	dotnet.ping(() => {
		dotnet.sendRequest('list-certs', null, (response, err) => {
			if (err) console.log(err);

			for (let cert of response) {

				var text = '';
				if (cert.isExpired) {
					text += '[EXPIRED] ';
				}
				text += cert.subjectName.commonName;
				if (!cert.isSelfSigned) {
					text += ` (issued by ${cert.issuerName.commonName})`
				}

				$('#signPdf #certificateSelect')
					.append($('<option>', {
						value: cert.thumbprintSHA256,
						text: text
					}));
			}
		});
	});
}

function signPdfChooseFile() {
	dialog.showOpenDialog({
		title: 'Choose a file to be signed',
		filters: [
			{ name: 'Documents', extensions: ['pdf'] },
			{ name: 'All Files', extensions: ['*'] }
		]
	}, filenames => {
		if (filenames === undefined) {
			console.log('No files were selected');
			return;
		}

		globalVariables.file = filenames[0];
		$('#chosenFile').text(path.basename(globalVariables.file));
		$('#chooseFileBtn')
			.empty()
			.text('Change File')
			.removeClass('btn-outline-dark')
			.addClass('btn-dark');
	});
}

// This function is called when the "Refresh Certificates" button is clicked.
function signPdfRefreshCerts() {
	resetCerts();
	signPdfInit();
}

function signPdf() {
	if (!globalVariables.file) {
		dialog.showMessageBox({
			message: 'Please choose a file to be signed!'
		});
		return;
	}
	let selectedCertificateThumbprint = $('#signPdf #certificateSelect').val();

	dotnet.ping(() => {
		dotnet.sendRequest('sign-pdf', {
			certThumb: selectedCertificateThumbprint,
			fileToSign: globalVariables.file
		}, (tempPath, err) => {

			if (err) {
				console.error(`An error has occurred: ${err}`);
				return;
			}

			dialog.showMessageBox(null, {
				type: 'none',
				title: 'Signature Succeeded!',
				buttons: ['Cancel', 'OK'],
				defaultId: 2,
				message: 'Your signature has finished with success!',
				detail: 'Press OK to save the file.'
			}, choice => {

				switch (choice) {
					case 1:
						dialog.showSaveDialog({
							title: 'Save signature file',
							defaultPath: globalVariables.file.replace(/.pdf$/, '-signed.pdf'),
							filters: [
								{ name: 'Documents', extensions: ['pdf'] },
								{ name: 'All Files', extensions: ['*'] }
							]
						}, path => {

							if (!path) {
								console.log('Signature has been discarted.');
								return;
							}

							moveFile(tempPath, path, err => {
								if (err) {
									console.error(`An error has occurred: ${err}`);
									return;
								}
			
								shell.openItem(path);
							});
						});
						break;
					default:
						console.log('Signature has been discarted.');
				}
			});

			
		});
	});
}

function moveFile(oldPath, newPath, callback) {

	fs.rename(oldPath, newPath, err => {
		 if (err) {
			  if (err.code === 'EXDEV') {
					var readStream = fs.createReadStream(oldPath);
					var writeStream = fs.createWriteStream(newPath);
		
					readStream.on('error', callback);
					writeStream.on('error', callback);
		
					readStream.on('close', function () {
						fs.unlink(oldPath, callback);
					});
		
					readStream.pipe(writeStream);
			  } else {
					callback(err);
			  }
			  return;
		 }
		 callback();
	});
}