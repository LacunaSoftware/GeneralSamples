using BirdIdSample.Classes;
using BirdIdSample.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BirdIdSample {
	public class Program {

		const string cpf = "**********";

		public async static Task Main(string[] args) {
			await recoverCertificateSample();
		}

		public async static Task recoverCertificateSample() {
			Console.WriteLine("*************************");
			Console.WriteLine("    List certificates    ");
			Console.WriteLine("*************************");

			string otp;
			do {
				Console.Write("Write your OTP: ");
				var res = Console.ReadLine();
				otp = res.Trim();
				if (string.IsNullOrEmpty(otp) || otp.Length != 6) {
					Console.WriteLine("Please, provided a valid OTP");
				}
			} while (string.IsNullOrEmpty(otp) || otp.Length != 6);

			var store = await BirdIdCertificateStore.LoadCertificatesAsync(cpf, otp);
		}
	}
}
