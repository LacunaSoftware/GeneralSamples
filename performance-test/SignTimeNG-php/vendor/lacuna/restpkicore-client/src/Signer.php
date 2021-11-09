<?php


namespace Lacuna\RestPki;
use DateTime;

/**
 * Class Signer
 * @package Lacuna\RestPki
 * 
 * @property DigestAlgorithmAndValue messageDigest
 * @property SignatureAlgorithmAndValue signature
 * @property DateTime signingTime
 * @property DateTime certifiedDateReference
 * @property ValidationResults validationResults
 * @property PKCertificate certificate
 * @property bool isDocumentTimestamp
 * @property string signatureFieldName
 */
class Signer
{
    public $messageDigest;
    public $signature;
    public $signingTime;
    public $certifiedDateReference;
    public $validationResults;
    public $certificate;
    public $isDocumentTimestamp;
    public $signatureFieldName;

    public function __construct($model)
    {
        if ($model->messageDigest) {
            $this->messageDigest = new DigestAlgorithmAndValue($model->messageDigest);
        }
        if ($model->signature) {
            $this->signature = new SignatureAlgorithmAndValue($model->signature);
        }
        if ($model->signingTime) {
            $this->signingTime = new DateTime($model->signingTime);
        }
        if ($model->certifiedDateReference) {
            $this->certifiedDateReference = new DateTime($model->certifiedDateReference);
        }
        if ($model->validationResults) {
            $this->validationResults = new ValidationResults($model->validationResults);
        }
        if ($model->certificate) {
            $this->certificate = new PKCertificate($model->certificate);
        }
        $this->isDocumentTimestamp = $model->isDocumentTimestamp;
        $this->signatureFieldName = $model->SignatureFieldName;
    }
}
