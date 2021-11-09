<?php


namespace Lacuna\RestPki;
use DateTime;

/**
 * Class SignerSummary
 * @package Lacuna\RestPki
 * 
 * @property-read CertificateSummary certificate
 * @property-read DateTime date
 */
class SignerSummary
{
    protected $_certificate;
    protected $_date;

    public function __construct($model)
    {
        if ($model->certificate) {
            $this->_certificate = new CertificateSummary($model->certificate);
        }
        if ($model->date) {
            $this->_date = new DateTime($model->date);
        }
    }


    protected function getCertificate()
    {
        return $this->_certificate;
    }
    protected function getDate()
    {
        return $this->_date;
    }

    public function __get($attr)
    {
        switch ($attr) {
            case "certificate":
                return $this->getCertificate();
            case "date":
                return $this->getDate();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $attr);
                return null;
        }
    }

    public function __isset($attr)
    {
        switch ($attr) {
            case "certificate":
                return isset($this->_certificate);
            case "date":
                return isset($this->_date);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $attr);
                return null;
        }
    }

}
