namespace DotnetCGI.Models {

	public class SignatureRequestModel {
		public string FileToSign { get; set; }
		public byte[] CertThumb { get; set; }
	}
}