<?php


namespace Lacuna\RestPki;

/**
 * Class DocumentFile
 * @package Lacuna\RestPki
 *
 * @property-read string $name
 * @property-read string $length
 * @property-read string $contentType
 * @property-read string $location
 */
class DocumentFile
{
    protected $_name;
    protected $_length;
    protected $_contentType;
    protected $_location;

    public function __construct($model)
    {
        $this->_name = $model->name;
        $this->_length = $model->length;
        $this->_contentType = $model->contentType;
        $this->_location = $model->location;
    }

    protected function getName(){
        return $this->_name;
    }

    protected function getLength(){
        return $this->_length;
    }

    protected function getContentType(){
        return $this->_contentType;
    }

    protected function getLocation(){
        return $this->_location;
    }

    public function __get($name)
    {
        switch ($name) {
            case "name":
                return $this->getName();
            case "length":
                return $this->getLength();
            case "contentType":
                return $this->getContentType();
            case "location":
                return $this->getLocation();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }

    public function __isset($name)
    {
        switch ($name) {
            case "name":
                return isset($this->_name);
            case "length":
                return isset($this->_length);
            case "contentType":
                return isset($this->_contentType);
            case "location":
                return isset($this->_location);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }
}
