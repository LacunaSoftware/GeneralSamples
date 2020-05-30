using iTextSharp.text;
using iTextSharp.text.pdf;
using Lacuna.Pki;
using Lacuna.Pki.Pades;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;

namespace ReceituarioSimples {

	/**
	 * This sample follows the standards created by ITI and CFM that demand
	 * that some fields (meta-data) to be added to the "Prescrição Eletrônica"
	 * document before it is signed.
	 * 
	 * The name of each field on this sample is based on the document provided
	 * by CFM on: https://prescricaoeletronica.cfm.org.br/. We will mark the
	 * required fields (meta-data) by the comment "REQUIRED!". Other fields
	 * are optional for the standard, but is necessary for the use of the
	 * document, such as the fields that compound the "Prescrição Eletrônica".
	 * 
	 * This sample uses the iTextSharp.LGPLv2.Core library to generate the
	 * document. It can be download by NuGet at:
	 * https://www.nuget.org/packages/iTextSharp.LGPLv2.Core/
	 * 
	 * References:
	 * - https://assinaturadigital.iti.gov.br/
	 * - https://prescricaoeletronica.cfm.org.br/
	 * - https://assinaturadigital.iti.gov.br/duvidas/#1587761771301-8f0416f4-c42c
	 */
	class Program {
		private static string nomeMedico = "João da Silva";
		private static string crm = "0000000";
		private static string crmUF = "DF";

		static void Main(string[] args) {
			// Create temporary file.
			var dest = Path.Join(Path.GetTempPath(), Guid.NewGuid() + ".pdf");

			// ********************************************************************
			//                        Generating Document
			// ********************************************************************
			Console.WriteLine($"Creating file {dest}");
			using (var stream = new FileStream(dest, FileMode.OpenOrCreate)) {
				var document = new Document();
				var writer = PdfWriter.GetInstance(document, stream);
				document.Open();

				// Add title.
				var title = new Paragraph("RECEITUÁRIO SIMPLES", new Font(null, 20f, Font.BOLD)) {
					Alignment = Element.ALIGN_CENTER,
				};
				document.Add(title);

				var table = new PdfPTable(6);
				table.WidthPercentage = 100;

				// REQUIRED!
				// Field "Tipo de Documento". This text field identifies the type of
				// document is being generated. It's a hidden field because this type
				// is identified by the field name and NOT by the value of this field.
				var tipoField = new PdfPCell() {
					Colspan = 6,
					Border = Rectangle.NO_BORDER,
				};
				tipoField.CellEvent = new TextFieldCellWrapper() {
					// See Enums.cs, to see what is the value of this enum below.
					FieldName = DocumentType.PrescricaoMedicamento.GetValue(),
					Value = string.Empty,
					ReadOnly = true,
					Hidden = true,
				};
				table.AddCell(tipoField);

				// Field "Nome do(a) Médico(a)".
				var doctorNameLabel = new PdfPCell() {
					Colspan = 2,
					Border = Rectangle.NO_BORDER,
				};
				doctorNameLabel.AddElement(new Phrase("NOME DO(A) MÉDICO(A):"));
				table.AddCell(doctorNameLabel);
				var doctorNameField = new PdfPCell() {
					Colspan = 4,
					Border = Rectangle.NO_BORDER,
				};
				doctorNameField.CellEvent = new TextFieldCellWrapper() {
					FieldName = FieldName.NomeCompletoEmitente.GetValue(),
					Value = nomeMedico,
					ReadOnly = false,
				};
				table.AddCell(doctorNameField);

				// REQUIRED!
				// Field "CRM". This text field contains the doctor's register
				// number on CRM. In this sample, we are filling in the field with
				// value of the variable "crm" defined above, that's why we set
				// this field as read-only.
				var crmLabel = new PdfPCell() {
					Colspan = 2,
					Border = Rectangle.NO_BORDER,
				};
				crmLabel.AddElement(new Phrase("CRM:"));
				table.AddCell(crmLabel);
				var crmField = new PdfPCell() {
					Colspan = 4,
					Border = Rectangle.NO_BORDER,
				};
				crmField.CellEvent = new TextFieldCellWrapper() {
					// See Enums.cs, to see what is the value of this enum below.
					FieldName = FieldName.Crm.GetValue(),
					Value = crm,
					ReadOnly = false,
				};
				table.AddCell(crmField);

				// REQUIRED!
				// Field "CRM UF". This combo box field contains the "UF" where the
				// doctor is registered. In this sample, we are filling in the field
				// with value of the variable "crmUF" defined above, that's why we
				// set this field as read-only.
				var crmUFLabel = new PdfPCell() {
					Colspan = 2,
					Border = Rectangle.NO_BORDER,
				};
				crmUFLabel.AddElement(new Phrase("CRM UF:"));
				table.AddCell(crmUFLabel);
				var crmUFField = new PdfPCell() {
					Colspan = 4,
					Border = Rectangle.NO_BORDER,
				};
				crmUFField.CellEvent = new ComboFieldCellWrapper() {
					// See Enums.cs, to see what is the value of this enum below.
					FieldName = FieldName.CrmUF.GetValue(),
					Options = new string[] { "AC", "AL", "AM", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MG", "MS", "MT", "PA", "PB", "PE", "PI", "PR", "RJ", "RN", "RO", "RR", "RS", "SC", "SE", "SP", "TO" },
					Selection = crmUF,
					ReadOnly = false,
				};
				table.AddCell(crmUFField);

				// Add table.
				document.Add(table);

				document.Close();
				writer.Close();
			}

			// ********************************************************************
			//                        Signing Document
			// ********************************************************************
			Console.WriteLine($"Signing file {dest}");

			// Instantiate PadesSigner.
			var padesSigner = new PadesSigner();

			// Set the generated file to be signed.
			padesSigner.SetPdfToSign(dest);

			// Provide the signer's certificate.
			padesSigner.SetSigningCertificate(Util.GetCertificateWithKey("Alan Mathison Turing.pfx", "1234"));

			// Use a policy accepted by ICP-Brasil.
			padesSigner.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic());

			// REQUIRED!
			// Use a custom signature field name. This field MUST have the 
			// "Emitente" keyword as the last keyword.
			padesSigner.SetCustomSignatureFieldName("Signature1 Emitente");

			// Compute the signature.
			padesSigner.ComputeSignature();

			// Store the signature file.
			var signedDocument = padesSigner.GetPadesSignature();
			File.WriteAllBytes(dest, signedDocument);

			// If you are on windows, you can uncomment the lines below to
			// automatically open the file with the default PDF reader.

			//Console.WriteLine($"Opening file {dest}");
			//Util.OpenWithDefaultProgram(dest);
		}

		/**
		 * This class is used to encapsulated a text field inside a table's cell.
		 * It is not mandadory to use this class in the sample, but it helps to
		 * create a grid to organize things into the PDF.
		 */
		class TextFieldCellWrapper : IPdfPCellEvent {
			public string FieldName { get; set; }
			public string Value { get; set; }
			public bool ReadOnly { get; set; } = default;
			public bool Hidden { get; set; } = default;

			public void CellLayout(PdfPCell cell, Rectangle rectangle, PdfContentByte[] canvases) {
				// Get cell writer.
				var writer = canvases[0].PdfWriter;

				// Create text field
				var field = new TextField(writer, rectangle, FieldName);
				field.Text = Value;

				// Configure Read-only option.
				if (ReadOnly) {
					field.Options = BaseField.READ_ONLY;
				}

				// Configure Read-only option.
				if (Hidden) {
					field.Visibility = BaseField.HIDDEN;
				}

				// Add text field.
				writer.AddAnnotation(field.GetTextField());
			}
		}

		/**
		 * This class is used to encapsulated a combo box field inside a table's
		 * cell. It is not mandadory to use this class in the sample, but it
		 * helps to create a grid to organize things into the PDF.
		 */
		class ComboFieldCellWrapper : IPdfPCellEvent {
			public string FieldName { get; set; }
			public string[] Options { get; set; }
			public string Selection { get; set; }
			public bool ReadOnly { get; set; } = default;
			public bool Hidden { get; set; } = default;

			public void CellLayout(PdfPCell cell, Rectangle rectangle, PdfContentByte[] canvases) {
				// Get cell writer.
				var writer = canvases[0].PdfWriter;

				// Create text field.
				var field = new TextField(writer, rectangle, FieldName);
				field.Choices = Options;
				field.ChoiceSelection = Array.IndexOf(Options, Selection);

				// Configure Read-only option.
				if (ReadOnly) {
					field.Options = BaseField.READ_ONLY;
				}

				// Configure Read-only option.
				if (Hidden) {
					field.Visibility = BaseField.HIDDEN;
				}

				// Add combo field.
				writer.AddAnnotation(field.GetComboField());
			}
		}
	}
}
