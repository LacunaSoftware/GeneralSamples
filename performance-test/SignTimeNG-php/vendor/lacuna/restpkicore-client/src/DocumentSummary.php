<?php


namespace Lacuna\RestPki;
use DateTime;

/**
 * Class Document
 * @package Lacuna\RestPki
 
 * @property-read string $id
 * @property-read string $key
 * @property-read string $formattedKey
 * @property-read DocumentFile $originalFile
 * @property-read DocumentFile $signedFile
 * @property-read DateTime $avaliableUntil
 * @property-read string $signatureType
 */
class DocumentSummary
{
    protected $_id;
    protected $_key;
    protected $_formattedKey;
    protected $_originalFile;
    protected $_signedFile;
    protected $_avaliableUntil;
    protected $_signatureType;

    public function __construct($model)
    {
        $this->_id = $model->id;
        $this->_key = $model->key;
        $this->_formattedKey = $model->formattedKey;
        if (isset($model->originalFile)) {
            $this->_originalFile = new DocumentFile($model->originalFile);
        }
        if (isset($model->signedFile)) {
            $this->_signedFile = new DocumentFile($model->signedFile);
        }
        if (isset($model->avaliableUntil)) {
            $this->_avaliableUntil = new DateTime($model->avaliableUntil);
        }
        $this->_signatureType = $model->signatureType;
    }

    /**
     * @return string The document's id.
     */
    protected function getId()
    {
        return $this->_id;
    }

    /**
     * @return string The document's key.
     */
    protected function getKey()
    {
        return $this->_key;
    }

    /**
     * @return string The document's formatted key.
     */
    protected function getFormattedKey()
    {
        return $this->_formattedKey;
    }

    /**
     * @return DocumentFile The original document.
     */
    protected function getOriginalFile()
    {
        return $this->_originalFile;
    }

    /**
     * @return DocumentFile The signed document.
     */
    protected function getSignedFile()
    {
        return $this->_signedFile;
    }

    /**
     * @return DateTime The last date that the document will be still avaliable.
     */
    protected function getAvaliableUntil()
    {
        return $this->_avaliableUntil;
    }

    /**
     * @return string The type of signature.
     */
    protected function getSignatureType()
    {
        return $this->_signatureType;
    }

    public function __get($attr)
    {
        switch ($attr) {
            case "id":
                return $this->getId();
            case "key":
                return $this->getKey();
            case "formattedKey":
                return $this->getFormattedKey();
            case "originalFile":
                return $this->getOriginalFile();
            case "signedFile":
                return $this->getSignedFile();
            case "avaliableUntil":
                return $this->getAvaliableUntil();
            case "signatureType":
                return $this->getSignatureType();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $attr);
                return null;
        }
    }

    public function __isset($attr)
    {
        switch ($attr) {
            case "id":
                return isset($this->_id);
            case "key":
                return isset($this->_key);
            case "formattedKey":
                return isset($this->_formattedKey);
            case "originalFile":
                return isset($this->_originalFile);
            case "signedFile":
                return isset($this->_signedFile);
            case "avaliableUntil":
                return isset($this->_avaliableUntil);
            case "signatureType":
                return isset($this->_signatureType);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $attr);
                return null;
        }
    }
}
