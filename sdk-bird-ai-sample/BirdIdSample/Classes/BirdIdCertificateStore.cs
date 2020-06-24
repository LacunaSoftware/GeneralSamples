using Lacuna.Pki;
using Lacuna.Pki.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BirdIdSample.Classes {
	public class BirdIdCertificateStore : ICertificateStore {

		const string clientId = "*************";
		const string clientSecret = "***************";
		const string endpoint = "https://apihom.birdid.com.br/v0";

		private MemoryCertificateStore _memoryCertificateStore;
		private List<PKCertificateWithKey> _certsWithKey;

		public string Cpf { get; set; }
		public string Otp { get; set; }
		public string Scope { get; set; }


		public BirdIdCertificateStore() {
			this._memoryCertificateStore = new MemoryCertificateStore();
			this._certsWithKey = new List<PKCertificateWithKey>();
		}

		private async Task initializeAsync(string cpf, string otp) {
			var client = new BirdIdClient(endpoint, clientId, clientSecret);
			var passwordAuthorizeResponse = await client.PasswordAuthorizeAsync(cpf, otp, scope: "signature_session");
			var certificateResponse = await client.GetCertificatesAsync(passwordAuthorizeResponse.AccessToken);

			foreach (var certificate in certificateResponse.Certificates) {
				var pkCertificate = PKCertificate.Decode(certificate.Certificate);
				this._memoryCertificateStore.Add(pkCertificate);

				var privateKey = new BirdIdPrivateKey() {
					Endpoint = endpoint,
					ClientId = clientId,
					ClientSecret = clientSecret,
					Alias = certificate.Alias,
					AccessToken = passwordAuthorizeResponse.AccessToken,
				};
				this._certsWithKey.Add(new PKCertificateWithKey(pkCertificate, privateKey));
			}
		}

		public async static Task<BirdIdCertificateStore> LoadCertificatesAsync(string cpf, string otp) {
			var store = new BirdIdCertificateStore();
			await store.initializeAsync(cpf, otp);
			return store;
		}

		public PKCertificate GetCertificate(Name issuerName, BigInteger serialNumber) {
			return _memoryCertificateStore.GetCertificate(issuerName, serialNumber);
		}

		public PKCertificate GetCertificate(byte[] keyIdentifier) {
			return _memoryCertificateStore.GetCertificate(keyIdentifier);
		}

		public List<PKCertificate> GetCertificates(Name subjectName) {
			return _memoryCertificateStore.GetCertificates(subjectName);
		}

		public List<PKCertificateWithKey> GetCertificatesWithKey() {
			return _certsWithKey;
		}
	}
}
