<?php


namespace Lacuna\RestPki;
use DateTime;

/**
 * Class SignatureSessionDocument
 * @package Lacuna\RestPki
 *
 * @property-read DocumentStatus $status
 * @property-read DataTime $dateSigned
 */
class SignatureSessionDocument extends DocumentSummary
{
    protected $_status;
    protected $_dateSigned;

    public function __construct($model)
    {
        parent::__construct($model);
        $this->_status = $model->status;
        if (isset($model->dateSigned)) {
            $this->_dateSigned = new DateTime($model->dateSigned);
        }
    }

    
    /**
     * @return DocumentStatus The document's status.
     */
    protected function getStatus()
    {
        return $this->_status;
    }

    /**
     * @return DataTime The document's signing date.
     */
    protected function getDateSigned()
    {
        return $this->_dateSigned;
    }

    public function __get($attr)
    {
        switch ($attr) {
            case "status":
                return $this->getStatus();
            case "dateSigned":
                return $this->getDateSigned();
            default:
                return parent::__get($attr);
        }
    }

    public function __isset($attr)
    {
        switch ($attr) {
            case "status":
                return isset($this->_status);
            case "dateSigned":
                return isset($this->_dateSigned);
            default:
                return parent::__get($attr);
        }
    }
}