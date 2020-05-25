using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultSample.Api.Signatures {
	public class SignatureRequest {
		public Guid CertId { get; set; }
		public string Pkcs12Password { get; set; }
	}
}
