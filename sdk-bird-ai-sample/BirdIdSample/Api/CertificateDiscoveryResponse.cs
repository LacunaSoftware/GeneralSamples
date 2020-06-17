using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdIdSample.Api {
	public class CertificateDiscoveryResponse {

		[JsonPropertyName("status")]
		public string Status { get; set; }

		[JsonPropertyName("certificates")]
		public List<CertificateModel> Certificates { get; set; }
	}

	public class CertificateModel {

		[JsonPropertyName("alias")]
		public string Alias { get; set; }

		[JsonPropertyName("certificate")]
		public string Certificate { get; set; }
	}
}
