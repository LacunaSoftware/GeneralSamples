using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PkiSdkNetCoreMVCSample.Classes;
using PkiSdkNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Hosting;
using Lacuna.Pki;
using Lacuna.Pki.Pades;

namespace PkiSdkNetCoreMVCSample.Controllers
{
	public class HomeController : Controller
	{
		private IWebHostEnvironment _env;
		public HomeController(IWebHostEnvironment env)
		{
			_env = env;
		}

		// Checks PKI SDK license
		public IActionResult Index()
		{
			if (!System.IO.File.Exists(Util.PkiLicensePath))
			{
				return View("PkiLicenseNotFound");
			}
			return RedirectToAction("BatchSignature");
		}

		private IPadesPolicyMapper GetSignaturePolicy()
		{

			// Get our custom trust arbitrator which accepts test certificates (see Util.GetTrustArbitrator()).
			var arbitrator = Util.GetTrustArbitrator();

			return PadesPoliciesForGeneration.GetPadesBasic(arbitrator);
		}

		/**
		 * This action renders the batch signature page.
		 *
		 * Notice that the only thing we'll do on the server-side at this point is determine the IDs of the
		 * documents to be signed. The page will handle each document one by one and will call the server
		 * asynchronously to start and complete each signature.
		 */
		public IActionResult BatchSignature()
		{
			// It is up to your application's business logic to determine which documents will compose the batch
			var model = new BatchSignatureModel()
			{
				DocumentIds = Enumerable.Range(1, 30).ToList() // from 1 to 30
			};
			// Render the batch signature page
			return View(model);
		}


		/**
		 * This controller contains the server-side logic for the optimized batch signature example.
		 *
		 * The logic for the example is more complex than the "regular" batch signature example (controller
		 * BatchSignatureController), but the performance is significantly improved (roughly 50% faster).
		 *
		 * Notice that the optimized batch example requires a use license for the Web PKI component (every other
		 * example in this project	does not). The licensing is not enforced when running on localhost, but in
		 * order to run this sample outside of localhost you'll need to set a license on the web.config file.
		 * If you need a trial license, please request one at https://www.lacunasoftware.com/en/products/web_pki
		 */

		/**
		 * We need to persist information about each batch in progress. For simplificy purposes, we'll store
		 * the information	about each batch on a static dictionary (server-side memory). If your application
		 * is stateless, you should persist this information on your database instead.
		 */
		private class BatchInfo
		{
			public string Certificate { get; set; }
		}
		private static Dictionary<Guid, BatchInfo> batches = new Dictionary<Guid, BatchInfo>();

		/**
		 * This action is called asynchronously to initialize a batch. We'll receive the user's certificate
		 * and store it (we'll need this information on each signature, but we'll avoid sending this
		 * repeatedly from the view in order to increase performance).
		 */
		[HttpPost]
		public IActionResult Init(BatchSignatureInitRequest request)
		{
			// Generate a unique ID identifying the batch.
			var batchId = Guid.NewGuid();
			// Store the user's certificate based on the generated ID.
			var batchInfo = new BatchInfo()
			{
				Certificate = request.Certificate,
			};
			lock (batches)
			{
				batches[batchId] = batchInfo;
			}

			// Return a JSON with the batch ID (the page will use jQuery to decode this value).
			return Json(new BatchSignatureInitResponse()
			{
				BatchId = batchId
			});
		}


		/**
		 * POST /BatchPadesSignatureSdk/Start
		 * 
		 * This action is called asynchronously from the batch signature page in order to initiate the signature
		 * of each document in the batch.
		 */
		[HttpPost]
		public IActionResult Start(BatchSignatureStartRequest request)
		{

			// Recover the batch information based on its ID, which contains the user's certificate.
			var batchInfo = batches[request.BatchId];

			byte[] toSignBytes, transferData;
			SignatureAlgorithm signatureAlg;

			request.CertContentBase64 = batchInfo.Certificate;
			// Decode the user's certificate
			var cert = PKCertificate.Decode(request.CertContent);

			// Instantiate a PadesSigner class
			var padesSigner = new PadesSigner();

			// Set the PDF to sign, which in the case of this example is one of the batch documents
			padesSigner.SetPdfToSign(StorageMock.GetBatchDocPath(request.DocumentId, _env));

			// Set the signer certificate
			padesSigner.SetSigningCertificate(cert);

			// Set the signature policy.
			padesSigner.SetPolicy(GetSignaturePolicy());

			// Set a visual representation for the signature.
			padesSigner.SetVisualRepresentation(PadesVisualElements.GetVisualRepresentationForPkiSdk(cert, _env));

			// Generate the "to-sign-bytes". This method also yields the signature algorithm that must
			// be used on the client-side, based on the signature policy, as well as the "transfer data",
			// a byte-array that will be needed on the next step.
			toSignBytes = padesSigner.GetToSignBytes(out signatureAlg, out transferData);

			// For the next steps, we'll need once again some information:
			// - The "transfer data" filename. Its content is stored in a temporary file (with extension .bin) to
			// be shared with the Complete action.
			// - The "to-sign-hash" (digest of the "to-sign-bytes"). And the OID of the digest algorithm to be 
			// used during the signature operation. this information is need in the signature computation with
			// Web PKI component. (see batch-signature-form.js)
			return Json(new BatchSignatureStartResponse()
			{
				TransferDataFileId = StorageMock.Store(transferData, _env, ".bin"),
				ToSignHash = signatureAlg.DigestAlgorithm.ComputeHash(toSignBytes),
				DigestAlgorithmOid = signatureAlg.DigestAlgorithm.Oid
			});
		}

		/**
		 * POST: /BatchPadesSignatureSdk/Complete
		 * 
		 * This action is called once the "to-sign-hash" are signed using the user's certificate. After signature,
		 * it'll be redirect to SignatureInfo action to show the signature file.
		 */
		[HttpPost]
		public IActionResult Complete(BatchSignatureCompleteRequest request)
		{
			byte[] signatureContent;

			// Recover the "transfer data" content stored in a temporary file.
			byte[] transferDataContent;
			if (!StorageMock.TryGetFile(request.TransferDataFileId, out transferDataContent, _env))
			{
				return NotFound();
			}

			// Instantiate a PadesSigner class
			var padesSigner = new PadesSigner();

			// Set the signature policy.
			padesSigner.SetPolicy(GetSignaturePolicy());

			// Set the signature computed on the client-side, along with the "transfer data" recovered from a temporary file
			padesSigner.SetPreComputedSignature(request.Signature, transferDataContent);

			// Call ComputeSignature(), which does all the work, including validation of the signer's certificate and of the 
			// resulting signature
			padesSigner.ComputeSignature();

			// Get the signed PDF as an array of bytes
			signatureContent = padesSigner.GetPadesSignature();

			return Json(new BatchSignatureCompleteResponse()
			{
				SignedFileId = StorageMock.Store(signatureContent, _env, ".pdf")
			});
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
