using Lacuna.Pki;
using Lacuna.Pki.AzureConnector;
using Lacuna.Pki.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultSample.Services {
	public class AzureKeyVaultStoreOptions {
		public string Endpoint { get; set; }
		public string AppId { get; set; }
		public string AppSecret { get; set; }
	}

	public class ImportCertificateResult {
		public Guid CertId { get; set; }
	}

	public class InvalidIdentifierException : Exception {
		internal InvalidIdentifierException() : base("The provided identifier is not valid") {}
	}

	public interface IAzureKeyVaultStore {
		Task<Guid> ImportPkcs12Async(byte[] pkcs12);
		Task<byte[]> GetPkcs12Async(Guid certId);
	}

	public class AzureKeyVaultStore : IAzureKeyVaultStore {

		private readonly IOptions<AzureKeyVaultStoreOptions> _options;
		private readonly BaseAzureApiAuthenticator _authenticator;

#pragma warning disable IDE1006 // Naming Styles

		private KeyVaultClient _client;
		private KeyVaultClient Client {
			get {
				if (_client == null) {
					_client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync));
				}
				return _client;
			}
		}
#pragma warning restore IDE1006 // Naming Styles

		public AzureKeyVaultStore(
			IOptions<AzureKeyVaultStoreOptions> options
		) {
			_options = options;
			_authenticator = new BaseAzureApiAuthenticator(options.Value);
		}

		public async Task<Guid> ImportPkcs12Async(byte[] pkcs12) {
			var options = _options.Value;

			// Store PKCS#12 on secrets.
			var certId = Guid.NewGuid();
			await Client.SetSecretAsync(options.Endpoint, certId.ToString(), Convert.ToBase64String(pkcs12));

			return certId;
		}

		public async Task<byte[]> GetPkcs12Async(Guid certId) {
			var options = _options.Value;

			// Retrieve the PKCS#12 file from secrets.
			SecretBundle secret;
			try {
				secret = await Client.GetSecretAsync(options.Endpoint, certId.ToString());
			} catch (KeyVaultErrorException ex) when (ex.Body?.Error?.Code == AzureErrorCodes.SecretNotFound) {
				throw new InvalidIdentifierException();
			}

			return Convert.FromBase64String(secret.Value);
		}

		public async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
			=> await _authenticator.GetAccessTokenAsync(authority, resource, scope);

		private static class AzureErrorCodes {
			public const string BadParameter = "BadParameter";
			public const string KeySizeNotSupported = "KeySizeNotSupported";
			public const string KeyNotFound = "KeyNotFound";
			public const string SecretNotFound = "SecretNotFound";
		}

		internal class BaseAzureApiAuthenticator : IAzureApiAuthenticator {
			private class CachedToken {
				public string Token { get; set; }
				public DateTimeOffset ExpiresOn { get; set; }
			}

			private static readonly Encoding _textEncoding = Encoding.UTF8;
			private static readonly TimeSpan _tokenValidityLimit = TimeSpan.FromMinutes(5);

			private readonly ConcurrentDictionary<string, CachedToken> _cachedTokens = new ConcurrentDictionary<string, CachedToken>();
			private readonly AzureKeyVaultStoreOptions _options;

			public BaseAzureApiAuthenticator(AzureKeyVaultStoreOptions options) {
				_options = options;
			}

			public async Task<string> GetAccessTokenAsync(string authority, string resource, string scope) {
				var cachedTokenId = string.Format(
					"{0}:{1}:{2}",
					 Convert.ToBase64String(_textEncoding.GetBytes(authority ?? "")),
					 Convert.ToBase64String(_textEncoding.GetBytes(resource ?? "")),
					 Convert.ToBase64String(_textEncoding.GetBytes(scope ?? ""))
				);

				if (_cachedTokens.TryGetValue(cachedTokenId, out CachedToken cachedToken) && DateTimeOffset.Now < cachedToken.ExpiresOn - _tokenValidityLimit) {
					return cachedToken.Token;
				}

				var result = await requestAccessTokenAsync(authority, resource, scope);

				if (result == null) {
					throw new InvalidOperationException("Failed to obtain the JWT token");
				}

				cachedToken = new CachedToken() {
					Token = result.AccessToken,
					ExpiresOn = result.ExpiresOn,
				};
				_cachedTokens.AddOrUpdate(cachedTokenId, cachedToken, (k, v) => cachedToken);

				return result.AccessToken;
			}

			private async Task<AuthenticationResult> requestAccessTokenAsync(string authority, string resource, string scope) {
				var authContext = new AuthenticationContext(authority);
				var clientCred = new ClientCredential(_options.AppId, _options.AppSecret);
				return await authContext.AcquireTokenAsync(resource, clientCred);
			}
		}
	}
}
