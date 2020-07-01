using Lacuna.Pki;

namespace DotnetCGI.Models {

	public class NameModel {

		public string Country { get; set; }
		public string Organization { get; set; }
		public string OrganizationUnit { get; set; }
		public string DNQualifier { get; set; }
		public string StateName { get; set; }
		public string CommonName { get; set; }
		public string SerialNumber { get; set; }
		public string Locality { get; set; }
		public string Title { get; set; }
		public string Surname { get; set; }
		public string GivenName { get; set; }
		public string Initials { get; set; }
		public string Pseudonym { get; set; }
		public string GenerationQualifier { get; set; }
		public string EmailAddress { get; set; }

		public NameModel(Name name) {
			this.CommonName = name.CommonName;
			this.Country = name.Country;
			this.DNQualifier = name.DNQualifier;
			this.EmailAddress = name.EmailAddress;
			this.GenerationQualifier = name.GenerationQualifier;
			this.GivenName = name.GivenName;
			this.Initials = name.Initials;
			this.Locality = name.Locality;
			this.Organization = name.Organization;
			this.OrganizationUnit = name.OrganizationUnit;
			this.Pseudonym = name.Pseudonym;
			this.SerialNumber = name.SerialNumber;
			this.StateName = name.StateName;
			this.Surname = name.Surname;
			this.Title = name.Title;
		}
	}
}