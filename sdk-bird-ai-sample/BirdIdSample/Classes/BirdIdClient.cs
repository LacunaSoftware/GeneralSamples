using BirdIdSample.Api;
using BirdIdSample.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BirdIdSample.Classes {
	public class BirdIdClient {
		public Uri EndpointUri { get; private set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }


		private HttpClient _client;
		public HttpClient Client {
			get {
				if (_client == null) {
					_client = new HttpClient();
				}
				return _client;
			}
		}

		public BirdIdClient(string endpointUri, string clientId, string clientSecret) : this(new Uri(endpointUri), clientId, clientSecret) {
		}

		public BirdIdClient(Uri endpointUri, string clientId, string clientSecret) {
			this.EndpointUri = endpointUri;
			this.ClientId = clientId;
			this.ClientSecret = clientSecret;
		}

		public async Task<PwdAuthorizeResponse> PasswordAuthorizeAsync(string username, string password, string scope = "authentication_session") {

			var requestJson = JsonSerializer.Serialize(new PwdAuthorizeRequest() {
				GrantType = "password",
				Scope = Uri.EscapeDataString(scope),
				ClientId = ClientId,
				ClientSecret = ClientSecret,
				Username = username, 
				Password = password,
			});

			Uri requestUrl;
			if (!Uri.TryCreate(this.EndpointUri, "/oauth/pwd_authorize", out requestUrl)) {
				throw new Exception("The provided URI is not valid");
			}

			var httpResponse = await Client.PostAsync(requestUrl, new StringContent(requestJson, Encoding.UTF8, "application/json"));
			if (!httpResponse.IsSuccessStatusCode) {
				var details = await httpResponse.Content.ReadAsStringAsync();
				throw new Exception(string.Format("An error has occurred while performing the request {0}: Error {1} - {2}", requestUrl.ToString(), httpResponse.StatusCode, details));
			}

			PwdAuthorizeResponse response;
			using (var stream = await httpResponse.Content.ReadAsStreamAsync()) {
				response = await JsonSerializer.DeserializeAsync<PwdAuthorizeResponse>(stream);
			}

			return response;
		}

		public async Task<CertificateDiscoveryResponse> GetCertificatesAsync(string bearerToken) {
			Uri requestUrl;
			if (!Uri.TryCreate(this.EndpointUri, "/oauth/certificate-discovery", out requestUrl)) {
				throw new Exception("The provided URI is not valid");
			}

			Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

			var httpResponse = await Client.GetAsync(requestUrl);
			if (!httpResponse.IsSuccessStatusCode) {
				var details = await httpResponse.Content.ReadAsStringAsync();
				throw new Exception(string.Format("An error has occurred while performing the request {0}: Error {1} - {2}", requestUrl.ToString(), httpResponse.StatusCode, details));
			}

			CertificateDiscoveryResponse response;
			using (var stream = await httpResponse.Content.ReadAsStreamAsync()) {
				response = await JsonSerializer.DeserializeAsync<CertificateDiscoveryResponse>(stream);
			}

			return response;
		}
		
		public async Task<SignatureResponse> SignHashAsync(byte[] hash, string digestAlgorithmOid, string alias, string bearerToken) {

			var requestJson = JsonSerializer.Serialize(new SignatureRequest() {
				CertificateAlias = alias,
				Hashes = new List<HashModel> () {
					new HashModel() {
						Id = "1",
						Alias = "Signature PKI SDK",
						Hash = hash,
						HashAlgorithm = digestAlgorithmOid,
					}
				}
			});

			Uri requestUrl;
			if (!Uri.TryCreate(this.EndpointUri, "/oauth/signature", out requestUrl)) {
				throw new Exception("The provided URI is not valid");
			}

			Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

			var httpResponse = await Client.PostAsync(requestUrl, new StringContent(requestJson, Encoding.UTF8, "application/json"));
			if (!httpResponse.IsSuccessStatusCode) {
				var details = await httpResponse.Content.ReadAsStringAsync();
				throw new Exception(string.Format("An error has occurred while performing the request {0}: Error {1} - {2}", requestUrl.ToString(), httpResponse.StatusCode, details));
			}

			SignatureResponse response;
			using (var stream = await httpResponse.Content.ReadAsStreamAsync()) {
				response = await JsonSerializer.DeserializeAsync<SignatureResponse>(stream);
			}

			return response;
		}
	}
}
