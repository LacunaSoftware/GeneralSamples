using BirdIdSample.Classes;
using BirdIdSample.Models;
using Lacuna.Pki;
using Lacuna.Pki.Pades;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BirdIdSample {
	public class Program {

		const string cpf = "**********";

		public async static Task Main(string[] args) {
			
			await signature();
		}
		public async static Task recoverCertificateSample() {
			Console.WriteLine("*************************");
			Console.WriteLine("    List certificates    ");
			Console.WriteLine("*************************");

			string otp;
			do {
				Console.Write("Write your OTP: ");
				var res = Console.ReadLine();
				otp = res.Trim();
				if (string.IsNullOrEmpty(otp) || otp.Length != 6) {
					Console.WriteLine("Please, provided a valid OTP");
				}
			} while (string.IsNullOrEmpty(otp) || otp.Length != 6);

			var store = await BirdIdCertificateStore.LoadCertificatesAsync(cpf, otp);
		}

		public async static Task signature() {
			Console.WriteLine("*************************");
			Console.WriteLine("         Signature       ");
			Console.WriteLine("*************************");

			string otp;
			do {
				Console.Write("Write your OTP: ");
				var res = Console.ReadLine();
				otp = res.Trim();
				if (string.IsNullOrEmpty(otp) || otp.Length != 6) {
					Console.WriteLine("Please, provided a valid OTP");
				}
			} while (string.IsNullOrEmpty(otp) || otp.Length != 6);

			var padesSigner = new PadesSigner();
			padesSigner.SetPdfToSign("SampleDocument.pdf");

			var trustArbitrator = new LinkedTrustArbitrator(TrustArbitrators.PkiBrazil, TrustArbitrators.Windows);
			var root = PKCertificate.Decode(Convert.FromBase64String("MIIFqjCCA5KgAwIBAgIIS9uixHDoFa8wDQYJKoZIhvcNAQENBQAwPTELMAkGA1UEBhMCQlIxEzARBgNVBAoTCklDUC1CcmFzaWwxGTAXBgNVBAMTEEFDIFJBSVogdGVzdGUgdjIwHhcNMTQwMjA0MTkwMDU3WhcNMjQwMjA0MTkwMDU3WjA9MQswCQYDVQQGEwJCUjETMBEGA1UEChMKSUNQLUJyYXNpbDEZMBcGA1UEAxMQQUMgUkFJWiB0ZXN0ZSB2MjCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKq/E45POudJct3XbsGSfeidHQmP3SAgKVt7URE3erLncOoIwEdgm2++cnSzuTqNSHo9F27eMtvvpsoizLUwEcZEwtkocijiN7FSLigdaJ/Ulb8ZNUcsvOHL82p+allTUCP9cJjmrkN6HwwtxfQGirvmP2Kq19mNJlJumTmD3w4Ar7pX1mi4kK7Fokud/000wrXydUfVcN3VWH2Nv0pioY+olsHi/AUXAfdlO4V7jtrWh3ZIlZJQIsZ0WNOS9NLxr4q7urubl2K23NAEVl5CCvVC3RJLCZdgH307y9fa7ajvKvrCls05T74aPTm0CoCVSQxP+L6UUIpz9U3aDTOILSfk8kx+aCN7K7XLZ5fG7ncIEy0JPgIv/UNftyXRrp4s+srG6oXR4fBf0komQ8UAk5tvdS3KNlfN/V4oLTnyQdK/hhH42hpCzHNlGTwomJKQnszFlozy1XuUYi0NuYZya+pqmG/GUkIDBoq6++W7rBHvZtjR1kjW9JkUTF12AvwwYXPWkUMJGWBUaWoyiSOf8fGoMp6aedhGBtGRFDTBFpNCBEj0lRVc2N0k+irOdTwsvrwhKQkeQhehULtNInsXfpUFmKKtfbQCym2ejmtXU4pCKI+3D2dc182Max+KIq3kAbK1FsjtSAtYUm5qlnWAYPufjWh60S1PS7ydu8WS+uyfAgMBAAGjga0wgaowHQYDVR0OBBYEFEc20cVEy8tlLaDGBYBZWVp2exxtMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAURzbRxUTLy2UtoMYFgFlZWnZ7HG0wRwYDVR0fBEAwPjA8oDqgOIY2aHR0cDovL2NjZC5hY3NvbHV0aS5jb20uYnIvdGVzdGVzL2FjLXJhaXotdGVzdGUtdjIuY3JsMA4GA1UdDwEB/wQEAwIBBjANBgkqhkiG9w0BAQ0FAAOCAgEAmhkzT4DC+5jRYkUuerr+jU6F4DNS+dxdLPFvIDVadLX1VmZ4SfNLiLKDyUZiP8DG0UAoa0lOjb6H8jLJV13bhKmQLYLOgjzAFOjXuTp4PjlbJ686J1f4dJV25ocNDjkX/z+I8OdaHjeuuv6lE5TjqztK18Y1wULFEGDgU7W81FyMOJB7+Ft1M88H+WVdkhY4nP4lHjCo4+vlq33WPwH9ov3GBGp74nTZko8AoygN2XB8csTC99LRlkmzQAZVsuBHM2GA0RGewv5YcLTc7cXb/JoIcZdWM2DepM3jAyBe3FLvaFgL/tPwOFNoppt0t0ctt0RMqOLRSncuHze75a1spjyNvAAfhy7uyn1+2+Vre7hVQnE8os6fmM0N/r5QCiQxOb7s08FwM2cUIHQ93Chxgjdil0RS9drUI2TQgHxkaGxQm1ufI4sUFj+R8e++P40sLWVjSMPU9jH3F8Stp/a+OCNao5wcr3OusmMUzmK6lEn7WUP0tJlWisoHlkpyWRiJO2LItwcOx4bggDGzSb0OmxP2SQEAx8yGbZ5HSMy655vp7oNyvj7336+1oxNPpC4a6rNwcLwPD+NwqO3U5VfqdzaxzIco8IRbuaoe2u/UXxuUwOUuwWvEOkbvdER9hdvhTcaN32mmA0Kect6WnDtU8JBxb7O8UQHjah7b6at9KWg="));
			trustArbitrator.Add(new TrustedRoots(root));
			padesSigner.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic(trustArbitrator));

			var store = await BirdIdCertificateStore.LoadCertificatesAsync(cpf, otp);
			var certs = store.GetCertificatesWithKey();
			padesSigner.SetSigningCertificate(certs.First());
			padesSigner.ComputeSignature();
			byte[] signedPdf = padesSigner.GetPadesSignature();
			File.WriteAllBytes(@"C:\temp\birdid-sample.pdf", signedPdf);
		}
	}
}
