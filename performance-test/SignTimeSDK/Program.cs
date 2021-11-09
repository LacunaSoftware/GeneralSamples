using Lacuna.Pki;
using Lacuna.Pki.Pades;
using Lacuna.Pki.Stores;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SignTimeSDK {
	class Program {
		static void Main(string[] args)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			PkiConfig.LoadLicense(Convert.FromBase64String("AxAAnCgd10a3Vk2twIfKMGq9WhEAUEtJIFN1aXRlIFNhbXBsZXMIAIAiBdzWO9kICAAA2I8rWW3ZCAAAAAAAAAQAfwAAAAABRofXZsPv3nGuaipEXM5az9s8Ha+wrywE7x++9950L8JodIxII6iEdB6o5wTnWZtKb4mUOX7EXr2bUgzWwMoXFQfyUzD9CkCQD+eaRoi4v+cInyWXKRMDpGuDC6CYcOMuZXO16kASewvhENTVlv0y7Kq5z9AyEuYyZvLV/tr+D2lF16S9j7KQacw9xZiXKkwlScxwr9x1xgoDSyXD6ngeX74LI2fqEYA1m3OvkR6yhbndU+BuAopgeKiwcJYHsk2Y6S7LPN0hx9gmqtdh2WaO5TK2B3P3/5o0qfd0VrEdPlbGTZnOPMK1p3sX+e/q4Q0/FrfyQrPR6OTRnlbBIlAZKw=="));
			var file5M = "D:/LacunaSoftware/Documentos/SampleTestes/Padrão/Lorem-5M.pdf";
			for (int i = 0; i < 100; i++)
			{
				PadesSDK(file5M);
			}

			stopWatch.Stop();
			// Get the elapsed time as a TimeSpan value.
			TimeSpan ts = stopWatch.Elapsed;

			// Format and display the TimeSpan value.
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				ts.Hours, ts.Minutes, ts.Seconds,
				ts.Milliseconds / 10);
			Console.WriteLine("RunTime " + elapsedTime);
		}

		public static ITrustArbitrator GetTrustArbitrator()
		{
			// We start by trusting the ICP-Brasil roots and the roots registered as trusted on the host
			// Windows Server.
			var trustArbitrator = new LinkedTrustArbitrator(TrustArbitrators.PkiBrazil, TrustArbitrators.Windows);
			// For development purposes, we also trust in Lacuna Software's test certificates.
			var lacunaRoot = PKCertificate.Decode(Convert.FromBase64String("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"));
			trustArbitrator.Add(new TrustedRoots(lacunaRoot));
			return trustArbitrator;
		}

		public static PadesVisualRepresentation2 GetVisualRepresentationForPkiSdk(PKCertificate certificate)
		{
			// Create a visual representation.
			var visualRepresentation = new PadesVisualRepresentation2()
			{

				// Text of the visual representation.
				Text = new PadesVisualText()
				{
					CustomText = String.Format("Signed by {0} ({1})", certificate.SubjectDisplayName, certificate.PkiBrazil.CPF),
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
					Container = new PadesVisualRectangle()
					{
						Left = 0.2,
						Top = 0.2,
						Right = 0.2,
						Bottom = 0.2
					}
				},
				Image = new PadesVisualImage()
				{
					// We'll use as background the image in Content/PdfStamp.png
					Content = File.ReadAllBytes("D:/LacunaSoftware/Projetos/PkiSuiteSamples/php/plain/resources/PdfStamp.png"),
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

		public static void PadesSDK(string userfile)
		{
			byte[] signatureContent;
			PKCertificateWithKey certWithKey;

			// Instantiate a CadesSigner class
			var padesSigner = new PadesSigner();

			// Set the file to be signed.
			padesSigner.SetPdfToSign(userfile);

			// Get the server's certificate, stores it and and set as the signer certificate.
			var certContent = File.ReadAllBytes("D:/LacunaSoftware/Certificados/Pierre de Fermat.pfx");
			var store = Pkcs12CertificateStore.Load(certContent, "1234");
			certWithKey = store.GetCertificatesWithKey().First();
			padesSigner.SetSigningCertificate(certWithKey);

			// Set the signature policy.
			padesSigner.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic(GetTrustArbitrator()));

			// Set a visual representation for the signature.
			padesSigner.SetVisualRepresentation(GetVisualRepresentationForPkiSdk(certWithKey.Certificate));

			var toSignBytes = padesSigner.GetToSignBytes(out var signatureAlg, out var transferData);

			byte[] signature = certWithKey.SignData(signatureAlg, toSignBytes);

			// Get an instance of the PadesSigner class.
			var padesSigner2 = new PadesSigner();

			// Set the signature policy.
			padesSigner2.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic(GetTrustArbitrator()));

			// Set the signature computed on the client-side, along with the "transfer data" (rendered in a hidden field, see the view)
			padesSigner2.SetPreComputedSignature(signature, transferData);

			// Call ComputeSignature(), which does all the work, including validation of the signer's certificate and of the resulting signature
			padesSigner2.ComputeSignature();

			// Get the signed PDF as an array of bytes
			signatureContent = padesSigner2.GetPadesSignature();

			using (var stream = new MemoryStream(signatureContent))
			{
				using (var fileStream = File.Create("SDK-signed.pdf"))
				{
					stream.CopyTo(fileStream);
				}
			}
		}
	}
}
