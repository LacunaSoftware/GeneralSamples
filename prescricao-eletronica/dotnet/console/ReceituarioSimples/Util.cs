using Lacuna.Pki;
using Lacuna.Pki.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ReceituarioSimples {
	public class Util {
		public static PKCertificateWithKey GetCertificateWithKey(string path, string password) {
			var pkcs12 = File.ReadAllBytes(path);
			var store = Pkcs12CertificateStore.Load(pkcs12, password);
			var certsWithKey = store.GetCertificatesWithKey();
			return certsWithKey.First();
		}

		public static void OpenWithDefaultProgram(string path) {
			Process fileopener = new Process();
			fileopener.StartInfo.FileName = "explorer";
			fileopener.StartInfo.Arguments = "\"" + path + "\"";
			fileopener.Start();
		}
	}
}
