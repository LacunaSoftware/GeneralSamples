using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdIdSample.Api {
	public class SignatureRequest {
		[JsonPropertyName("certificate_alias")]
		public string CertificateAlias { get;set; }
		[JsonPropertyName("hashes")]
		public List<HashModel> Hashes { get; set; }
		[JsonPropertyName("include_chain")]
		public bool IncludeChain { get; set; } = false;
	}

	public class HashModel {
		[JsonPropertyName("id")]
		public string Id { get; set; }
		[JsonPropertyName("alias")]
		public string Alias { get; set; }
		[JsonPropertyName("hash")]
		public byte[] Hash { get; set; }
		[JsonPropertyName("hash_algorithm")]
		public string HashAlgorithm { get; set; }
		[JsonPropertyName("signature_format")]
		public string SignatureFormat { get; set; } = "RAW";
	}
}
