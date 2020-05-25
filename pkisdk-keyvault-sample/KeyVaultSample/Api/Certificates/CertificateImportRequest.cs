using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultSample.Api.Certificate {
	public class CertificateImportRequest {
		public byte[] Pkcs12 { get; set; }
		public string Pkcs12Password { get; set; }
	}
}
