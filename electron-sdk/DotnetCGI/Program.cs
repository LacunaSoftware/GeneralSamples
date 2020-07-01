using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotnetCGI.Models;
using ElectronCgi.DotNet;
using Lacuna.Pki;
using Lacuna.Pki.Pades;
using Lacuna.Pki.Stores;

namespace DotnetCGI {
	class Program {
		static void Main(string[] args) {
			// This is a TRIAL token. It will expire at 31/08/2020.
			PkiConfig.LoadLicense(Convert.FromBase64String("AxAAIIy8jc59Q0q95BZrL57K5hEAUEtJIFN1aXRlIFNhbXBsZXMIAAD0Ze31HdgICACAXwryrU7YCAAAAAAAAAQAfwAAAAABL2+ls7EW5LHD/tEetd49d0JpmU7pXEjhH0pU1ZSp5qjvKxL8c8PZz6ODTf68+lfQtXkKaRlQH6hu7VTSU3fvhCmZovDB5ruKqJPn+MQRDBbS8Wkr/meVo9LBS+3NFOky+EY43ebFoFxTbVZl2lCjb0DuskJiZGuHOBJ1v2XpGdKCmh1c1LmMvpc+OPegzNuMCXoEzSN9DdRtKnDzRxvOnvPglCX9+oV89LWsmVzonRp1a+tluqa8Ron9pFdHI9cWBElcXpmwXbKbmP0Sy5yYbYpE+rYsNgD5sV/FwF8uOxGWA0/mRWLZlO3OcGWoYo7qBBDmCUApAcRmZR3tXqhELQ=="));
			
			var connection = new ConnectionBuilder()
								 .WithLogging()
								 .Build();

			// "List Certificates" operation.
			connection.On<string, List<CertificateModel>>("list-certs", _ => {
				var store = WindowsCertificateStore.LoadPersonalCurrentUser();

				return store.GetCertificatesWithKey().Select(c => new CertificateModel(c.Certificate)).ToList();
			});

			// "Sign a PDF" operation.
			connection.On<SignatureRequestModel, string>("sign-pdf", request => {
				var signer = new PadesSigner();

				var store = WindowsCertificateStore.LoadPersonalCurrentUser();
				var signingCert = store.GetCertificatesWithKey().First(c => c.Certificate.ThumbprintSHA256.SequenceEqual(request.CertThumb));

				signer.SetSigningCertificate(signingCert);
				signer.SetPdfToSign(request.FileToSign);

				var trustArbitrator = new LinkedTrustArbitrator(TrustArbitrators.PkiBrazil, TrustArbitrators.Windows);
				// For development purposes, we also trust in Lacuna Software's test certificates.
				var lacunaRoot = Lacuna.Pki.PKCertificate.Decode(Convert.FromBase64String("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"));
				// COMMENT the line below before production release
				trustArbitrator.Add(new TrustedRoots(lacunaRoot));

				signer.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic(trustArbitrator));
				signer.ComputeSignature();

				byte[] signedPdf = signer.GetPadesSignature();

				var tempLocation = Path.GetTempFileName();
				File.WriteAllBytes(tempLocation, signedPdf);

				return tempLocation;
			});

			// Acknowledges that the connection is running.
			connection.On<string, string>("ping", argument => "pong");


			// wait for incoming requests
			connection.Listen();
		}
	}
}
