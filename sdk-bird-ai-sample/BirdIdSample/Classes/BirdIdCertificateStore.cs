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
		const string authorizationEndpoint = "https://apihom.birdid.com.br/v0";

		private MemoryCertificateStore _memoryCertificateStore;

		public string Cpf { get; set; }
		public string Otp { get; set; }
		public string Scope { get; set; }


		public BirdIdCertificateStore() {
			this._memoryCertificateStore = new MemoryCertificateStore();
		}

		private async Task initializeAsync(string cpf, string otp, string scope) {
			var client = new BirdIdClient(authorizationEndpoint, clientId, clientSecret);
			var passwordAuthorizeResponse = await client.PasswordAuthorizeAsync(cpf, otp, scope);
			var certificateResponse = await client.GetCertificatesAsync(passwordAuthorizeResponse.AccessToken);

			var certificates = certificateResponse.Certificates.Select(c => PKCertificate.Decode(c.Certificate)).ToList();
			this._memoryCertificateStore.AddRange(certificates);
		}


		public async static Task<BirdIdCertificateStore> LoadCertificatesAsync(string cpf, string otp, string scope = "authentication_session") {
			var store = new BirdIdCertificateStore();
			await store.initializeAsync(cpf, otp, scope);
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
	}
}
