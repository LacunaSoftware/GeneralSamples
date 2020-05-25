using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyVaultSample.Api;
using KeyVaultSample.Api.Certificate;
using KeyVaultSample.Services;
using Lacuna.Pki;
using Lacuna.Pki.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;

namespace KeyVaultSample.Controllers {

	[Route("api/certificates")]
	[ApiController]
	public class CertificatesController : ControllerBase {
		private readonly IAzureKeyVaultStore _azureKeyVaultStore;

		public CertificatesController(IAzureKeyVaultStore azureKeyVaultStore) {
			_azureKeyVaultStore = azureKeyVaultStore;
		}

		[HttpPost]
		[Route("import")]
		public async Task<IActionResult> Import([FromBody] CertificateImportRequest request) {

			// 1. Open PKCS#12, verifying valid of the provided password.
			Pkcs12CertificateStore store;
			try {
				store = Pkcs12CertificateStore.Load(request.Pkcs12, request.Pkcs12Password);
			} catch (IncorrectPinException ex) {
				return UnprocessableEntity(new ErrorModel() {
					Code = ErrorCodes.InvalidPIN,
					Message = ex.Message,
				});
			}

			// 2. Retrieve certification info.
			var certs = store.GetCertificatesWithKey();
			if (!certs.Any()) {
				return UnprocessableEntity(new ErrorModel() {
					Code = ErrorCodes.InvalidPkcs12,
					Message = "The provided PKCS#12 file is not valid",
				});
			}
			var cert = certs.First();

			// Certificate infomation.
			var subjectName = cert.Certificate.SubjectName.CommonName;
			var cpf = cert.Certificate.PkiBrazil.CPF;
			var cpfFormatted = cert.Certificate.PkiBrazil.CpfFormatted;
			var cnpj = cert.Certificate.PkiBrazil.Cnpj;
			var cnpjFormatted = cert.Certificate.PkiBrazil.CnpjFormatted;
			var dateOfBirth = cert.Certificate.PkiBrazil.DateOfBirth;

			// 3. Store certificate at Azure KeyVault.
			var certId = await _azureKeyVaultStore.ImportPkcs12Async(request.Pkcs12);

			// 4. Store certId in a database and associate it with the user account.
			// %%% EXAMPLE %%%
			//_businessService.Save(new User() {
			//	CertId = certId,
			//	Id = cpf,
			//});
			// %%%%%%%%%%%%%%%

			return Ok(new CertificateImportResponse() {
				CertId = certId,
			});
		}
	}
}