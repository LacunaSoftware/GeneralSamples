<?php


namespace Lacuna\RestPki;

/**
 * Class SignatureSession
 * @package Lacuna\RestPki
 * 
 * @property-read string $id
 * @property-read string $status
 * @property-read string $processingErrorCode
 * @property-read string $callbackArgument
 * @property-read PKCertificate $signerCertificate
 * @property-read SignatureSessionDocument[] $documents
 */
class SignatureSession
{
    protected $_id;
    protected $_status;
    protected $_callbackArgument;
    protected $_processingErrorCode;
    protected $_signerCertificate;
    protected $_documents;

    public function __construct($model)
    {
        $this->_id = $model->id;
        $this->_status = $model->status;
        $this->_callbackArgument = $model->callbackArgument;
        $this->_processingErrorCode = $model->processingErrorCode;

        if($model->signerCertificate){
            $this->_signerCertificate = new PKCertificate($model->signerCertificate);
        }

        if($model->documents) {
            $this->_documents = array();
            foreach ($model->documents as $documentModel) {
                array_push($this->_documents, new SignatureSessionDocument($documentModel));
            }
        }
    }

    protected function getId(){
        return $this->_id;
    }

    protected function getStatus(){
        return $this->_status;
    }

    protected function getCallbackArgument(){
        return $this->_callbackArgument;
    }

    protected function getProcessingErrorCode(){
        return $this->_processingErrorCode;
    }

    protected function getSignerCertificate(){
        return $this->_signerCertificate;
    }

    protected function getDocuments(){
        return $this->_documents;
    }

    public function __get($name)
    {
        switch ($name) {
            case "id":
                return $this->getId();
            case "status":
                return $this->getStatus();
            case "callbackArgument":
                return $this->getCallbackArgument();
            case "processingErrorCode":
                return $this->getProcessingErrorCode();
            case "signerCertificate":
                return $this->getSignerCertificate();
            case "documents":
                return $this->getDocuments();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }

    public function __isset($name)
    {
        switch ($name) {
            case "id":
                return isset($this->_id);
            case "status":
                return isset($this->_status);
            case "callbackArgument":
                return isset($this->_callbackArgument);
            case "processingErrorCode":
                return isset($this->_processingErrorCode);
            case "signerCertificate":
                return isset($this->_signerCertificate);
            case "documents":
                return isset($this->_documents);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }
}
