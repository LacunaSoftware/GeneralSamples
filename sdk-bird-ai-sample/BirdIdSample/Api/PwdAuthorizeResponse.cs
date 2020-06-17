using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdIdSample.Models {
	public class PwdAuthorizeResponse {

		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("expires_in")]
		public string ExpiresIn { get; set; }

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }

		[JsonPropertyName("scope")]
		public string Scope { get; set; }
	}
}
