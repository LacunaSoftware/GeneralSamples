using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdIdSample.Api {
	public class SignatureResponse {
		[JsonPropertyName("certificate_alias")]
		public string CertificateAlias { get; set; }
		[JsonPropertyName("signatures")]
		public List<SignatureModel> Signatures { get; set; }
	}

	public class SignatureModel {
		[JsonPropertyName("id")]
		public string Id { get; set; }
		[JsonPropertyName("raw_signature")]
		public byte[] RawSignature { get; set; }
	}
}
