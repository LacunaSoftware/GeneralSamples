using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeyVaultSample.Api;
using KeyVaultSample.Api.Signatures;
using KeyVaultSample.Services;
using Lacuna.Pki;
using Lacuna.Pki.Pades;
using Lacuna.Pki.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KeyVaultSample.Controllers {

	[Route("api/signatures")]
	[ApiController]
	public class SignaturesController : ControllerBase {
		private readonly IAzureKeyVaultStore _azureKeyVaultStore;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public SignaturesController(
			IAzureKeyVaultStore azureKeyVaultStore,
			IWebHostEnvironment webHostEnvironment
		) {
			_azureKeyVaultStore = azureKeyVaultStore;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] SignatureRequest request) {

			// 1. Retrieve key using certId stored on your database.
			byte[] pkcs12;
			try {
				pkcs12 = await _azureKeyVaultStore.GetPkcs12Async(request.CertId);
			} catch (InvalidIdentifierException ex) {
				return UnprocessableEntity(new ErrorModel() {
					Code = ErrorCodes.InvalidIdentifier,
					Message = ex.Message,
				});
			}

			// 2. Open PKCS#12, verifying valid of the provided password.
			Pkcs12CertificateStore store;
			try {
				store = Pkcs12CertificateStore.Load(pkcs12, request.Pkcs12Password);
			} catch (IncorrectPinException ex) {
				return UnprocessableEntity(new ErrorModel() {
					Code = ErrorCodes.InvalidPIN,
					Message = ex.Message,
				});
			}

			// 3. Retrieve certification info (include its key).
			var certs = store.GetCertificatesWithKey();
			if (!certs.Any()) {
				return UnprocessableEntity(new ErrorModel() {
					Code = ErrorCodes.InvalidPkcs12,
					Message = "The provided PKCS#12 file is not valid",
				});
			}
			var cert = certs.First();

			// 4. Perform signature.
			var signer = new PadesSigner();

			signer.SetSigningCertificate(cert);
			signer.SetPdfToSign(Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "SamplePdf.pdf"));
			signer.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic());
			signer.SetVisualRepresentation(GetVisualRepresentation(cert.Certificate));
			signer.ComputeSignature();
			byte[] signedPdf = signer.GetPadesSignature();

			// 5. Store signature file.
			if (!System.IO.File.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data"))) {
				Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data"));
			}
			var fileId = Guid.NewGuid() + ".pdf";
			System.IO.File.WriteAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", fileId), signedPdf);

			return Ok(new SignatureResponse() {
				FileId = fileId,
			});
		}

		// This function is called by the PAdES samples for PKI SDK. It contains a example of signature visual
		// representation. This is only a separate function in order to organize the variaous examples.
		public PadesVisualRepresentation2 GetVisualRepresentation(PKCertificate cert) {

			// Create a visual representation.
			var visualRepresentation = new PadesVisualRepresentation2() {

				// Text of the visual representation.
				Text = new PadesVisualText() {
					CustomText = String.Format("Signed by {0} ({1})", cert.SubjectDisplayName, cert.PkiBrazil.CPF),
					FontSize = 13.0,
					// Specify that the signing time should also be rendered.
					IncludeSigningTime = true,
					// Optionally set the horizontal alignment of the text ('Left' or 'Right'), if not set the
					// default is Left.
					HorizontalAlign = PadesTextHorizontalAlign.Left,
					// Optionally set the container within the signature rectangle on which to place the
					// text. By default, the text can occupy the entire rectangle (how much of the rectangle the
					// text will actually fill depends on the length and font size). Below, we specify that text
					// should respect a right margin of 1.5 cm.
					Container = new PadesVisualRectangle() {
						Left = 0.2,
						Top = 0.2,
						Right = 0.2,
						Bottom = 0.2
					}
				},
				Image = new PadesVisualImage() {
					// We'll use as background the image in Content/PdfStamp.png
					Content = System.IO.File.ReadAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "PdfStamp.png")),
					// Align image to the right horizontally.
					HorizontalAlign = PadesHorizontalAlign.Right,
					// Align image to center vertically.
					VerticalAlign = PadesVerticalAlign.Center
				}
			};

			// Position of the visual representation. We get the footnote position preset and customize it.
			var visualPositioning = PadesVisualAutoPositioning.GetFootnote();
			visualPositioning.Container.Height = 4.94;
			visualPositioning.SignatureRectangleSize.Width = 8.0;
			visualPositioning.SignatureRectangleSize.Height = 4.94;
			visualRepresentation.Position = visualPositioning;

			return visualRepresentation;
		}
	}
	
}