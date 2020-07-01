using System;
using System.Numerics;
using Lacuna.Pki;

namespace DotnetCGI.Models {

	public class CertificateModel {
		public NameModel SubjectName { get; set; }
		public NameModel IssuerName { get; set; }
		public bool IsExpired { get; set; }
		public bool IsSelfSigned { get; set; }
		public byte[] ThumbprintSHA256 { get; set; }

		public CertificateModel(PKCertificate cert) {
			if (cert.SubjectName != null) {
				this.SubjectName = new NameModel(cert.SubjectName);
			}
			if (cert.IssuerName != null) {
				this.IssuerName = new NameModel(cert.IssuerName);
			}
			if (cert.ValidityEnd != null) {
				this.IsExpired = DateTime.Now > cert.ValidityEnd;
			}

			this.IsSelfSigned = cert.IsSelfSigned;
			this.ThumbprintSHA256 = cert.ThumbprintSHA256;
		}
	}
}