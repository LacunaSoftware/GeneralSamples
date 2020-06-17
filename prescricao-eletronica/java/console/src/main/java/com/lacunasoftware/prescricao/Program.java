package com.lacunasoftware.prescricao;

import com.itextpdf.text.*;
import com.itextpdf.text.Font;
import com.itextpdf.text.Rectangle;
import com.itextpdf.text.pdf.BaseFont;
import com.itextpdf.text.pdf.PdfPCell;
import com.itextpdf.text.pdf.PdfPTable;
import com.itextpdf.text.pdf.PdfWriter;
import com.lacunasoftware.pkiexpress.PadesSigner;
import com.lacunasoftware.pkiexpress.StandardSignaturePolicies;

import java.awt.*;
import java.io.*;
import java.net.URI;
import java.net.URISyntaxException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.UUID;

public class Program {
	private static String nomeMedico = "Jo\u00E3o da Silva";
	private static String crm = "0000000";
	private static String crmUF = "DF";

	public static void main(String[] args) throws IOException, DocumentException {
		// Create temporary file.
		Path tempFolder = Paths.get(System.getProperty("java.io.tmpdir"));
		Path dest = tempFolder.resolve(UUID.randomUUID() + ".pdf");

		// ********************************************************************
		//                        Generating Document
		// ********************************************************************
		System.out.println(String.format("Creating file %s", dest));
		try (FileOutputStream fos = new FileOutputStream(dest.toFile())) {
			Document document = new Document();
			PdfWriter writer = PdfWriter.getInstance(document, fos);
			document.open();

			// Add title.
			Paragraph title = new Paragraph("RECEITU\u00C1RIO SIMPLES", new Font((BaseFont) null, 20f, Font.BOLD));
			title.setSpacingAfter(5f);
			title.setAlignment(Element.ALIGN_CENTER);
			document.add(title);

			PdfPTable table = new PdfPTable(6);
			table.setWidthPercentage(100);

			// REQUIRED!
			// Field "Tipo de Documento". This text field identifies the type of
			// document is being generated. It's a hidden field because this type
			// is identified by the field name and NOT by the value of this field.
			PdfPCell tipoField = new PdfPCell();
			tipoField.setColspan(6);
			tipoField.setBorder(Rectangle.NO_BORDER);
			TextFieldCellWrapper tipoCellWrapper = new TextFieldCellWrapper();
			// See DocumentType.java, to see what is the vale of this enum below.
			tipoCellWrapper.setFieldName(DocumentType.PRESCRICAO_MEDICAMENTO.getValue());
			tipoCellWrapper.setValue("");
			tipoCellWrapper.setReadOnly(true);
			tipoCellWrapper.setHidden(true);
			tipoField.setCellEvent(tipoCellWrapper);
			table.addCell(tipoField);

			// Field "Nome do(a) Médico(a)".
			PdfPCell doctorNameLabel = new PdfPCell();
			doctorNameLabel.setColspan(2);
			doctorNameLabel.setBorder(Rectangle.NO_BORDER);
			doctorNameLabel.addElement(new Phrase("NOME DO(A) M\u00C9DICO(A):"));
			table.addCell(doctorNameLabel);
			PdfPCell doctorNameField = new PdfPCell();
			doctorNameField.setColspan(4);
			doctorNameField.setBorder(Rectangle.NO_BORDER);
			TextFieldCellWrapper doctorNameCellWrapper = new TextFieldCellWrapper();
			doctorNameCellWrapper.setFieldName("03_Nome Completo Emitente");
			doctorNameCellWrapper.setValue(nomeMedico);
			doctorNameCellWrapper.setReadOnly(true);
			doctorNameField.setCellEvent(doctorNameCellWrapper);
			table.addCell(doctorNameField);

			// REQUIRED!
			// Field "CRM". This text field contains the doctor's register
			// number on CRM. In this sample, we are filling in the field with
			// value of the variable "crm" defined above, that's why we set
			// this field as read-only.
			PdfPCell crmLabel = new PdfPCell();
			crmLabel.setColspan(2);
			crmLabel.setBorder(Rectangle.NO_BORDER);
			crmLabel.addElement(new Phrase("CRM:"));
			table.addCell(crmLabel);
			PdfPCell crmField = new PdfPCell();
			crmField.setColspan(4);
			crmField.setBorder(Rectangle.NO_BORDER);
			TextFieldCellWrapper crmCellWrapper = new TextFieldCellWrapper();
			// See FieldName.java, to see what is the value of this enum below.
			crmCellWrapper.setFieldName(FieldName.CRM.getValue());
			crmCellWrapper.setValue(crm);
			crmCellWrapper.setReadOnly(true);
			crmField.setCellEvent(crmCellWrapper);
			table.addCell(crmField);

			// REQUIRED!
			// Field "CRM UF". This combo box field contains the "UF" where the
			// doctor is registered. In this sample, we are filling in the field
			// with value of the variable "crmUF" defined above, that's why we
			// set this field as read-only.
			PdfPCell crmUFLabel = new PdfPCell();
			crmUFLabel.setColspan(2);
			crmUFLabel.setBorder(Rectangle.NO_BORDER);
			crmUFLabel.addElement(new Phrase("CRM UF:"));
			table.addCell(crmUFLabel);
			PdfPCell crmUFField = new PdfPCell();
			crmUFField.setColspan(1);
			crmUFField.setBorder(Rectangle.NO_BORDER);
			ComboFieldCellWrapper crmUFCellWrapper = new ComboFieldCellWrapper();
			// See FieldName.java, to see what is the value of this enum below.
			crmUFCellWrapper.setFieldName(FieldName.CRM_UF.getValue());
			crmUFCellWrapper.setOptions(new String[] { "AC", "AL", "AM", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MG", "MS", "MT", "PA", "PB", "PE", "PI", "PR", "RJ", "RN", "RO", "RR", "RS", "SC", "SE", "SP", "TO" });
			crmUFCellWrapper.setSelection(crmUF);
			crmUFCellWrapper.setReadOnly(true);
			crmUFField.setCellEvent(crmUFCellWrapper);
			table.addCell(crmUFField);
			PdfPCell empty = new PdfPCell();
			empty.setColspan(3);
			empty.setBorder(Rectangle.NO_BORDER);
			table.addCell(empty);

			// Add table.
			document.add(table);

			document.close();
			writer.close();
		}

		// ********************************************************************
		//                        Signing Document
		// ********************************************************************
		System.out.println(String.format("Signing file %s", dest));

		// Instantiate PadesSigner
		PadesSigner signer = new PadesSigner();

		// Set PDF to be signed.
		signer.setPdfToSign(dest);

		// REQUIRED!
		// Use a policy accepted by ICP-Brasil.
		signer.setSignaturePolicy(StandardSignaturePolicies.PadesBasicWithLTV);

		// REQUIRED!
		// Provide the signer's certificate. You must sign with a valid digital
		// certificate of a doctor, who was registered on CRM. In this sample,
		// we used a sample certificate stored on server to do the execute this
		// sample.
		try (InputStream stream = openSampleCertificate()) {
			signer.setPkcs12(stream);
		}
		signer.setCertPassword("1234");

		// REQUIRED!
		// Use a custom signature field name. This field MUST have the
		// "Emitente" keyword as the last keyword.
		signer.setCustomSignatureFieldName("Signature1 Emitente");

		// Overwrite original file with the signed file.
		signer.setOverwriteOriginalFile(true);

		// Perform the signature.
		signer.sign();

		// ********************************************************************
		//                     Showing Signed Document
		// ********************************************************************
		System.out.println(String.format("Opening file %s", dest));
		Desktop.getDesktop().open(dest.toFile());
	}

	public static InputStream openSampleCertificate() {
		FileInputStream fis;
		try {
			URI uri = new URI(Program.class.getResource("/static/Pierre de Fermat.pfx").getFile());
			fis = new FileInputStream(uri.getPath());
		} catch (FileNotFoundException|URISyntaxException ex) {
			throw new RuntimeException("Sample certificate couldn't be opened");
		}
		return fis;
	}
}
