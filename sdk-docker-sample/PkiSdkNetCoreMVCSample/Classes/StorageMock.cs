using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PkiSdkNetCoreMVCSample.Classes
{
	public class StorageMock
	{

		public static bool TryGetFile(string fileId, out byte[] content, IWebHostEnvironment env)
		{
			content = null;

			if (string.IsNullOrEmpty(fileId))
			{
				return false;
			}
			var filename = fileId.Replace('_', '.');
			// Note: we're receiving the fileId argument with "_" as "." because of limitations of ASP.NET MVC.

			var path = Path.Combine(env.WebRootPath, "App_Data", filename);
			var fileInfo = new FileInfo(path);
			if (!fileInfo.Exists)
			{
				return false;
			}
			content = File.ReadAllBytes(path);
			return true;
		}

		public static Stream OpenRead(string filename, IWebHostEnvironment env)
		{

			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("fileId");
			}

			var path = Path.Combine(env.WebRootPath, "App_Data", filename);
			var fileInfo = new FileInfo(path);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException("File not found: " + filename);
			}
			return fileInfo.OpenRead();
		}

		public static byte[] Read(string fileId, IWebHostEnvironment env)
		{

			if (string.IsNullOrEmpty(fileId))
			{
				throw new ArgumentNullException("fileId");
			}
			var filename = fileId.Replace("_", ".");
			// Note: we're receiving the fileId argument with "_" as "." because of limitations of ASP.NET MVC.

			using (var inputStream = OpenRead(filename, env))
			{
				using (var buffer = new MemoryStream())
				{
					inputStream.CopyTo(buffer);
					return buffer.ToArray();
				}
			}
		}

		public static byte[] Read(string fileId, IWebHostEnvironment env, out string filename)
		{

			if (string.IsNullOrEmpty(fileId))
			{
				throw new ArgumentNullException("fileId");
			}
			filename = fileId.Replace("_", ".");
			// Note: we're receiving the fileId argument with "_" as "." because of limitations of ASP.NET MVC.

			using (var inputStream = OpenRead(filename, env))
			{
				using (var buffer = new MemoryStream())
				{
					inputStream.CopyTo(buffer);
					return buffer.ToArray();
				}
			}
		}

		public static string Store(Stream stream, IWebHostEnvironment env, string extension = "", string filename = null)
		{

			// Guarantees that the "App_Data" folder exists.
			if (!Directory.Exists(Path.Combine(env.WebRootPath, "App_Data")))
			{
				Directory.CreateDirectory(Path.Combine(env.WebRootPath, "App_Data"));
			}

			if (string.IsNullOrEmpty(filename))
			{
				filename = Guid.NewGuid() + extension;
			}

			var path = Path.Combine(env.WebRootPath, "App_Data", filename.Replace("_", "."));

			using (var fileStream = File.Create(path))
			{
				stream.CopyTo(fileStream);
			}

			return filename.Replace(".", "_");
			// Note: we're passing the filename argument with "." as "_" because of limitations of ASP.NET MVC.
		}

		public static string Store(byte[] content, IWebHostEnvironment env, string extension = "", string filename = null)
		{
			string fileId;
			using (var stream = new MemoryStream(content))
			{
				fileId = Store(stream, env, extension, filename);
			}
			return fileId;
		}

		public static byte[] GetPdfStampContent(IWebHostEnvironment env)
		{
			return File.ReadAllBytes(Path.Combine(env.WebRootPath, "PdfStamp.png"));
		}

		public static string GetBatchDocPath(int id, IWebHostEnvironment env)
		{
			return Path.Combine(env.WebRootPath, "sampleFiles", string.Format("{0:D2}.pdf", id % 10));
		}

	}
}