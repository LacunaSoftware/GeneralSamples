<?php
require __DIR__ . '/vendor/autoload.php';

use Lacuna\PkiExpress\StandardSignaturePolicies;
use Lacuna\PkiExpress\PadesSigner;

$nomeMedico = "João da Silva";
$crm =  '0000000';
$crmUF = 'DF';
$ufs = array('AC', 'AL', 'AM', 'AP', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MG', 'MS', 'MT', 
'PA', 'PB', 'PE', 'PI', 'PR', 'RJ', 'RN', 'RO', 'RR', 'RS', 'SC', 'SE', 'SP', 'TO');

$dest = sys_get_temp_dir()."\\".uniqid().".pdf";

// ********************************************************************
//                        Generating Document
// ********************************************************************
echo "Generating file ".$dest;

// create new PDF document
$pdf = new TCPDF(PDF_PAGE_ORIENTATION, PDF_UNIT, PDF_PAGE_FORMAT, true, 'UTF-8', false);

// set margins
$pdf->SetMargins(PDF_MARGIN_LEFT, PDF_MARGIN_TOP, PDF_MARGIN_RIGHT);

// remove default header/footer
$pdf->setPrintHeader(false);
$pdf->setPrintFooter(false);

// set font
$pdf->SetFont('helvetica', '', 10, '', false);

// add a page
$pdf->AddPage();
// ---------------------------------------------------------

// set default form properties
$pdf->setFormDefaultProp(array('lineWidth'=>1, 'borderStyle'=>'solid', 'fillColor'=>array(255, 255, 200), 'strokeColor'=>array(255, 128, 128)));

// Add form field
// REQUIRED!
// Field "Tipo de Documento". This text field identifies the type of
// document is being generated. It's a hidden field because this type
// is identified by the field name and NOT by the value of this field.
$pdf->TextField(DocumentType::PRESCRICAO_MEDICAMENTO, 0, 0, array('hidden' => 'true', 'readonly' => 'true', 'value'=>''));

$pdf->SetFont('helvetica', 'BI', 20);
$pdf->Cell(0, 5, 'RECEITUÁRIO SIMPLES', 0, 1, 'C');
$pdf->Ln(10);

$pdf->SetFont('helvetica', '', 12);

// Name
$pdf->Cell(55, 7, 'NOME DO(A) MÉDICO(A):');
$pdf->TextField('03_Nome Completo Emitente', 100, 7, array('readonly' => 'true', 'value'=> $nomeMedico));
$pdf->Ln(8);

// REQUIRED!
// Field "CRM". This text field contains the doctor's register
// number on CRM. In this sample, we are filling in the field with
// value of the variable "crm" defined above, that's why we set
// this field as read-only.
$pdf->Cell(55, 7, 'CRM:');
$pdf->TextField(FieldName::CRM, 100, 7, array('readonly' => 'true', 'value'=> $crm));
$pdf->Ln(8);

// REQUIRED!
// Field "CRM UF". This combo box field contains the "UF" where the
// doctor is registered. 
$pdf->Cell(55, 7, 'CRM UF:');
$pdf->ComboBox(FieldName::CRM_UF, 100, 7, $ufs, array('value'=> $crmUF));

// ---------------------------------------------------------

//Close and output PDF document
$pdf->Output($dest, 'F');

echo "\nSigning file ".$dest;

$signer = new PadesSigner();
$signer->signaturePolicy = StandardSignaturePolicies::PADES_BASIC;
$signer->setPdfToSign($dest);
$signer->setPkcs12(getcwd()."\\Alan Mathison Turing.pfx");
$signer->setCertPassword("1234");
$signer->setOutputFile($dest);
$signer->setCustomSignatureFieldName("Signature1 Emitente");

// Uncomment for test purpose only
// $signer->trustLacunaTestRoot = true;

$signer->sign();

echo "\nSigned file ".$dest;
?>
