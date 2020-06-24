using Lacuna.Pki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BirdIdSample.Classes {
	public class BirdIdPrivateKey : IPrivateKey {
		public string Endpoint { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Alias { get; set; }
		public string AccessToken { get; set; }

		public ISignatureCsp GetSignatureCsp(DigestAlgorithm digestAlgorithm) {
			return new BirdIdSignatureCsp() {
				Endpoint = Endpoint,
				ClientId = ClientId,
				ClientSecret = ClientSecret,
				Alias = Alias,
				Digest = digestAlgorithm,
				AccessToken = AccessToken,
			};
		}
	}

	public class BirdIdSignatureCsp : ISignatureCsp {
		public string Endpoint { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Alias { get; set; }
		public DigestAlgorithm Digest { get; set; }
		public string AccessToken { get; set; }

		public bool CanSign => true;

		public byte[] SignData(byte[] buffer) {
			return SignData(buffer, 0, buffer.Length);
		}

		public byte[] SignData(Stream inputStream) {
			byte[] signature;
			using (var memoryStream = new MemoryStream()) {
				inputStream.CopyTo(memoryStream);
				signature = SignData(memoryStream.ToArray());
			}
			return signature;
		}

		public byte[] SignData(byte[] buffer, int offset, int count) {
			var client = new BirdIdClient(Endpoint, ClientId, ClientSecret);
			var signHashResponse = client.SignHashAsync(Digest.ComputeHash(buffer, offset, count), Digest.Oid, Alias, AccessToken).GetAwaiter().GetResult();
			return signHashResponse.Signatures[0].RawSignature;
		}

		public byte[] SignHash(byte[] hash) {
			var client = new BirdIdClient(Endpoint, ClientId, ClientSecret);
			var signHashResponse = client.SignHashAsync(hash, Digest.Oid, Alias, AccessToken).GetAwaiter().GetResult();
			return signHashResponse.Signatures[0].RawSignature;
		}

		public bool VerifyData(byte[] dataSigned, byte[] signature) {
			throw new NotImplementedException();
		}

		public bool VerifyHash(byte[] hashSigned, byte[] signature) {
			throw new NotImplementedException();
		}
	}
}
