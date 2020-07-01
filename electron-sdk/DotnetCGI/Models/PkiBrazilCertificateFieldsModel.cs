using System;
using Lacuna.Pki;

namespace DotnetCGI.Models {

	public class PkiBrazilCertificateFieldsModel {

		public string Cpf { get; set; }

		public string Cnpj { get; set; }

		public string Responsavel { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string CompanyName { get; set; }

		public string OabUF { get; set; }

		public string OabNumero { get; set; }

		public string RGEmissor { get; set; }

		public string RGEmissorUF { get; set; }

		public string RGNumero { get; set; }

		public PkiBrazilCertificateFieldsModel(IcpBrasilCertificateFields pkiBrazil) {
			this.Cpf = pkiBrazil.CPF;
			this.Cnpj = pkiBrazil.Cnpj;
			this.Responsavel = pkiBrazil.Responsavel;
			this.DateOfBirth = pkiBrazil.DateOfBirth;
			this.CompanyName = pkiBrazil.CompanyName;
			this.OabUF = pkiBrazil.OabUF;
			this.OabNumero = pkiBrazil.OabNumero;
			this.RGNumero = pkiBrazil.RGNumero;
			this.RGEmissor = pkiBrazil.RGEmissor;
			this.RGEmissorUF = pkiBrazil.RGEmissorUF;
		}
	}
}