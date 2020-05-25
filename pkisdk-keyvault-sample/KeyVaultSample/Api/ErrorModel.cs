using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultSample.Api {
	public class ErrorModel {
		public ErrorCodes Code { get; set; }
		public string Message { get; set; }
	}
}
