#!/usr/bin/env node
const path = require('path');
const uuid = require('uuid');
const os = require('os');
const { PadesSigner,
        StandardSignaturePolicies,
} = require('pki-express');

const { GeneratePrescricao } = require('./generate-prescricao');

const data = {
    nomeMedico: 'João da Silva',
    crm: '0000000',
    crmUF: 'DF'
}

// Create temporary file.
let dest = path.join(os.tmpdir(), `${uuid.v4()}.pdf`);

// ********************************************************************
//                        Generating Document
// ********************************************************************
console.log(`Creating file ${dest}`);
GeneratePrescricao(dest, data);

// ********************************************************************
//                        Generating Document
// ********************************************************************

console.log(`Signing file ${dest}`);

// Instantiate PadesSigner.
var padesSigner = new PadesSigner();

// Set the generated file to be signed.
padesSigner.setPdfToSignFromPathSync(dest);

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

// // REQUIRED!
// // Use a custom signature field name. This field MUST have the 
// // "Emitente" keyword as the last keyword.
// padesSigner.SetCustomSignatureFieldName("Signature1 Emitente");

padesSigner.outputFile = dest;

// Compute the signature.
padesSigner.sign().then();