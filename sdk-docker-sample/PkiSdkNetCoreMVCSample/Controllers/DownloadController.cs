using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PkiSdkNetCoreMVCSample.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PkiSdkNetCoreMVCSample.Controllers
{

	/**
	 * This controller's purpose is to download the sample file that is signed during the signature examples
	 * or download a upload file for signature or download a previously performed signature.
	 */
	public class DownloadController : Controller
	{


		public IWebHostEnvironment _env;

		public DownloadController(IWebHostEnvironment env)
		{
			_env = env;
		}

		// GET Download/File/{id}
		[HttpGet]
		public ActionResult File(string id)
		{
			byte[] content;

			if (id == null)
			{
				return NotFound();
			}

			string filename;
			try
			{
				content = StorageMock.Read(id, _env, out filename);
			}
			catch (FileNotFoundException)
			{
				return NotFound();
			}

			return File(content, "application/pdf", filename);
		}

		// GET Download/Doc/{id}
		[HttpGet]
		public ActionResult Doc(int id)
		{
			var fileContent = StorageMock.Read(StorageMock.GetBatchDocPath(id, _env), _env);
			return File(fileContent, "application/pdf", string.Format("Doc{0:D2}.pdf", id));
		}

	}
}