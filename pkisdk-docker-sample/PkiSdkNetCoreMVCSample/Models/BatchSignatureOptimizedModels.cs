using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PkiSdkNetCoreMVCSample.Models
{
	public class BatchSignatureInitRequest
	{
		public string Certificate { get; set; }
	}
	public class BatchSignatureInitResponse
	{
		public Guid BatchId { get; set; }
	}
	public class BatchSignatureStartRequest
	{
		public Guid BatchId { get; set; }
		public byte[] CertContent { get; set; }
		public string CertContentBase64 {
			get {
				return CertContent != null ? Convert.ToBase64String(CertContent) : "";
			}
			set {
				CertContent = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : null;
			}
		}
		public int DocumentId { get; set; }
	}
	public class BatchSignatureStartResponse
	{
		public string Token { get; set; }
		public byte[] ToSignHash { get; set; }
		public string DigestAlgorithmOid { get; set; }

		public string TransferDataFileId { get; set; }
	}
	public class BatchSignatureCompleteRequest
	{
		public byte[] Signature { get; set; }
		public string SignatureBase64 {
			get {
				return Signature != null ? Convert.ToBase64String(Signature) : "";
			}
			set {
				Signature = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : null;
			}
		}

		public string TransferDataFileId { get; set; }
	}

	public class BatchSignatureCompleteResponse
	{
		public string SignedFileId { get; set; }
	}
}
