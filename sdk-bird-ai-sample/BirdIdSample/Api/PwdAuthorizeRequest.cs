using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdIdSample.Models {
	public class PwdAuthorizeRequest {

		[JsonPropertyName("grant_type")]
		public string GrantType { get; set; }
		[JsonPropertyName("scope")]
		public string Scope { get; set; }
		[JsonPropertyName("client_id")]
		public string ClientId { get; set; }
		[JsonPropertyName("client_secret")]
		public string ClientSecret { get; set; }
		[JsonPropertyName("username")]
		public string Username { get; set; }
		[JsonPropertyName("password")]
		public string Password { get; set; }
		[JsonPropertyName("lifetime")]
		public string Lifetime { get; set; }
	}
}
