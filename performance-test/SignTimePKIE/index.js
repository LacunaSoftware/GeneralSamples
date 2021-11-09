const {
	PadesSignatureStarter,
	StandardSignaturePolicies,
	SignatureFinisher,
	PadesSigner,
} = require('pki-express');
const exec = require('await-exec')

main = async function(){
    const start = Date.now()
    for (let i = 0; i < 100; i++) {
        await signPades2("D:/LacunaSoftware/Documentos/SampleTestes/Padrão/Lorem-5M.pdf");
    }
    
    const stop = Date.now()
    return `Time Taken to execute = ${(stop - start)/1000} seconds`;
};

main2 = async function(filename) {
    filename = "D:/LacunaSoftware/Documentos/SampleTestes/Padrão/Lorem-5M.pdf";
    p1 = 0;
    p2 = 0;
    for (let i = 0; i < 2; i++) {
        p1start = Date.now()
        const signatureStarter = new PadesSignatureStarter();
        setPkiDefaults(signatureStarter);
        // Set signature policy.
        signatureStarter.signaturePolicy = StandardSignaturePolicies.PADES_BASIC_WITH_LTV;

        // Set PDF to be signed.
        signatureStarter.setPdfToSignFromPathSync(filename);

        // Set Base64-encoded certificate's content to signature starter.
        signatureStarter.setCertificateFromPathSync("D:/LacunaSoftware/Certificados/Pierre de Fermat.pfx");
        signatureStarter.setCertificateFromBase64Sync("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");

        response = await signatureStarter.start();
        const { toSignHash } = response;
        const { transferFile } = response;

        p1stop = Date.now();
        p1 += (p1stop - p1start)/1000;

        signature = await signHash(toSignHash);

        p2start = Date.now();
        const signatureFinisher = new SignatureFinisher();
        setPkiDefaults(signatureFinisher);
        signatureFinisher.setFileToSignFromPathSync(filename);
        signatureFinisher.setTransferFileFromPathSync(transferFile);
        signatureFinisher.signature = signature['stdout'].replace('\n', '');
        const outputFile = `PKIE-signed.pdf`;
        signatureFinisher.outputFile = outputFile;
        await signatureFinisher.complete(false);
        p2stop = Date.now();
        p2 += (p2stop - p2start)/1000;
    }
    return `P1 = ${p1} seconds | P2 = ${p2} seconds`;
}

signHash = async function(toSignHash) {
    command = 'pkie sign-hash ' + toSignHash + ' -t faea8507c5056effe0bba75851b496503b190340';
    return await exec(command);
}

signPades = async function(filename) {
    // Get an instance of the PadesSigner class, responsible for receiving
    // the signature elements and performing the local signature.
    const signer = new PadesSigner();

    setPkiDefaults(signer);

    // Set signature policy.
    signer.signaturePolicy = StandardSignaturePolicies.PADES_BASIC_WITH_LTV;

    // Set PDF to be signed.
    await signer.setPdfToSignFromPath(filename);

    // Set sample PKCS #12 path.
    await signer.setPkcs12FromPath("D:/LacunaSoftware/Certificados/Pierre de Fermat.pfx");
    signer.certPassword = '1234';

    // await signer.addFileReference('stamp', StorageMock.getPdfStampPath());
    // await signer.setVisualRepresentation(PadesVisualElementsExpress.getVisualRepresentation());

    // Generate path for output file and add the signature finisher.
    const outputFile = `PKIE-signed.pdf`;
    signer.outputFile = outputFile;
    await signer.sign();
}

setPkiDefaults = function(operator) {
    operator.trustLacunaTestRoot = true;
};

main2()
    .then(text => {
        console.log(text);
    })
    .catch(err => {
        console.error(err);
    });