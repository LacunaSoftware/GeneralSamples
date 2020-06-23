#!/usr/bin/env node
const path = require('path');
const uuid = require('uuid');
const os = require('os');
const fs = require('fs');

const PDFDocument = require('pdfkit');
const { PadesSigner,
    StandardSignaturePolicies,
} = require('pki-express');

const { DocumentType, FieldName } = require('./enums');

const data = {
    nomeMedico: 'João da Silva',
    crm: '0000000',
    crmUF: 'DF'
};

// Create temporary file.
let dest = path.join(os.tmpdir(), `${uuid.v4()}.pdf`);

// ********************************************************************
//                        Generating Document
// ********************************************************************
console.log(`Creating file ${dest}`);
const doc = new PDFDocument;

// write to PDF
// doc.pipe(fs.createWriteStream(dest));

// Add title.
doc.fontSize(20);
doc.font('Times-Bold')
    .text('RECEITUÁRIO SIMPLES', {align: 'center'})
    .moveDown(1);

// Initialize Form
doc.fontSize(12).font('Helvetica');
doc.initForm();

// Add field labels
doc.text('NOME DO(A) MÉDICO(A):', {align: 'left'}).moveDown(1);
doc.text('CRM:', {align: 'left'}).moveDown(1);
doc.text('CRM UF:', {align: 'left'});

// Add form field
// REQUIRED!
// Field "Tipo de Documento". This text field identifies the type of
// document is being generated. It's a hidden field because this type
// is identified by the field name and NOT by the value of this field.
doc.formText(DocumentType.PrescricaoMedicamento, 0, 0, 0, 0, {
    value: '',
    readOnly: true,
});

let y = 112.5;
doc.formText('03_Nome Completo Emitente', 240, y, 300, 25, {
    value: data.nomeMedico,
    align: 'left',
    readOnly: true,
});

// REQUIRED!
// Field "CRM". This text field contains the doctor's register
// number on CRM. In this sample, we are filling in the field with
// value of the variable "crm" defined above, that's why we set
// this field as read-only.
y += 25;
doc.formText(FieldName.Crm, 240, y, 300, 25, {
    value: data.crm,
    align: 'left',
    readOnly: true,
});

// REQUIRED!
// Field "CRM UF". This combo box field contains the "UF" where the
// doctor is registered. In this sample, we are filling in the field
// with value of the variable "crmUF" defined above, that's why we
// set this field as read-only.
y += 25;
opts = {
    select: ['AC', 'AL', 'AM', 'AP', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MG', 'MS', 'MT',
    'PA', 'PB', 'PE', 'PI', 'PR', 'RJ', 'RN', 'RO', 'RR', 'RS', 'SC', 'SE', 'SP', 'TO'],
    value: data.crmUF,
    align: 'left',
    readOnly: true,
};
doc.formCombo(FieldName.CrmUF, 240, y, 300, 25, opts);

doc.end();

let bufs = [];
doc.on('data', (d) => bufs.push(d));
doc.on('end', function () {
    let buffer = Buffer.concat(bufs);
    // ********************************************************************
    //                        Generating Document
    // ********************************************************************

    console.log(`Signing file ${dest}`);

    fs.writeFileSync('C:\\temp\\test-prescrição.pdf', buffer);

    // Instantiate PadesSigner.
    const padesSigner = new PadesSigner();

    // Set the generated file to be signed.
    padesSigner.setPdfToSignFromRawSync(buffer);

    // REQUIRED!
    // Provide the signer's certificate. You must sign with a valid digital
    // certificate of a doctor, who was registered on CRM. In this sample,
    // we used a sample certificate stored on server to do the execute this
    // sample.
    padesSigner.setPkcs12FromPathSync(path.join(process.cwd(), 'Alan Mathison Turing.pfx'));
    padesSigner.certPassword = '1234';

    // REQUIRED!
    // Use a policy accepted by ICP-Brasil.
    padesSigner.signaturePolicy = StandardSignaturePolicies.PADES_BASIC;

    // Uncomment for test purpose only
    // padesSigner.trustLacunaTestRoot = true;

    // REQUIRED!
    // Use a custom signature field name. This field MUST have the
    // "Emitente" keyword as the last keyword.
    padesSigner.customSignatureFieldName = "Signature1 Emitente";

    padesSigner.outputFile = dest;

    // Compute the signature.
    padesSigner.sign().then(() => {
        console.log("Signature Finished!");
    });
});

