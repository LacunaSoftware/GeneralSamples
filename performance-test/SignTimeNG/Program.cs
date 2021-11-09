using Lacuna.Pki;
using Lacuna.Pki.Stores;
using Lacuna.RestPki.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignTimeNG {
	class Program {
		public  static void Main()
		{
			PkiConfig.LoadLicense(Convert.FromBase64String("AxAAnCgd10a3Vk2twIfKMGq9WhEAUEtJIFN1aXRlIFNhbXBsZXMIAIAiBdzWO9kICAAA2I8rWW3ZCAAAAAAAAAQAfwAAAAABRofXZsPv3nGuaipEXM5az9s8Ha+wrywE7x++9950L8JodIxII6iEdB6o5wTnWZtKb4mUOX7EXr2bUgzWwMoXFQfyUzD9CkCQD+eaRoi4v+cInyWXKRMDpGuDC6CYcOMuZXO16kASewvhENTVlv0y7Kq5z9AyEuYyZvLV/tr+D2lF16S9j7KQacw9xZiXKkwlScxwr9x1xgoDSyXD6ngeX74LI2fqEYA1m3OvkR6yhbndU+BuAopgeKiwcJYHsk2Y6S7LPN0hx9gmqtdh2WaO5TK2B3P3/5o0qfd0VrEdPlbGTZnOPMK1p3sX+e/q4Q0/FrfyQrPR6OTRnlbBIlAZKw=="));

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			var file5M = "D:/LacunaSoftware/Documentos/SampleTestes/Padrão/Lorem-5M.pdf";
			for (int i = 0; i < 100; i++)
			{
				PadesRestNG(file5M).GetAwaiter().GetResult();
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

		public static Lacuna.Pki.DigestAlgorithm[] possibleAlgs = new Lacuna.Pki.DigestAlgorithm[] {
					Lacuna.Pki.DigestAlgorithm.MD5,
					Lacuna.Pki.DigestAlgorithm.SHA1,
					Lacuna.Pki.DigestAlgorithm.SHA256,
					Lacuna.Pki.DigestAlgorithm.SHA384,
					Lacuna.Pki.DigestAlgorithm.SHA512,
				};

		public static async Task PadesRestNG(string userfile) 
		{
			var fileRef = FileReference.FromStream(File.OpenRead(userfile), userfile);

			var certContent = File.ReadAllBytes("D:/LacunaSoftware/Certificados/Pierre de Fermat.pfx");
			X509Certificate2 cert = new X509Certificate2(certContent, "1234");

			var options = new RestPkiOptions
			{
				Endpoint = "https://localhost:44325/",
				ApiKey = "Testes Locais|72923cbd7337474d82f1003781f2a6b329eadfb2f776851b928bcdd31c6073d9"
			};

			var service = RestPkiServiceFactory.GetService(options);

			//var certRef = Lacuna.RestPki.Client.CertificateReference.FromBytes(File.ReadAllBytes("D:/LacunaSoftware/Certificados/Pierre de Fermat.pfx"));
			var certRef = Lacuna.RestPki.Client.CertificateReference.FromX509Certificate2(cert);

			var prepareResult = await service.PrepareSignatureAsync(fileRef, certRef);

			// Get the server's certificate, stores it and and set as the signer certificate.
			var store = Pkcs12CertificateStore.Load(certContent, "1234");
			var certWithKey = store.GetCertificatesWithKey().First();

			var hash = prepareResult.ToSignHash;

			var digestAlg = possibleAlgs.FirstOrDefault(a => a.Name == hash.Algorithm.Name);
			byte[] signature = certWithKey.SignHash(digestAlg, hash.Value);

			var complete = await service.CompleteSignatureAsync(prepareResult.State, signature);
		}
	}
}
