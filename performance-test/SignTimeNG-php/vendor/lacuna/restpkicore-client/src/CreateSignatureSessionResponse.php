<?php


namespace Lacuna\RestPki;

/**
 * Class CreateSignatureSessionResponse
 * @package Lacuna\RestPki
 *
 * @property-read string $sessionId
 * @property-read string $redirectUrl
 */
class CreateSignatureSessionResponse
{
    private $_sessionId;
    private $_redirectUrl;

    public function __construct($model)
    {
        $this->_sessionId = $model->sessionId;
        $this->_redirectUrl = $model->redirectUrl;
    }

    public function getSessionId(){
        return $this->_sessionId;
    }

    public function getRedirectUrl(){
        return $this->_redirectUrl;
    }

    public function __get($name)
    {
        switch ($name) {
            case "sessionId":
                return $this->getSessionId();
            case "redirectUrl":
                return $this->getRedirectUrl();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }

    public function __isset($name)
    {
        switch ($name) {
            case "sessionId":
                return isset($this->_sessionId);
            case "redirectUrl":
                return isset($this->_redirectUrl);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $name);
                return null;
        }
    }
}