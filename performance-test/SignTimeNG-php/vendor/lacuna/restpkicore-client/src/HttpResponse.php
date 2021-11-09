<?php

namespace Lacuna\RestPki;

use Psr\Http\Message\ResponseInterface;

/**
 * Class HttpResponse
 * @package Lacuna\RestPki
 *
 * @property $body mixed
 * @property $statusCode int
 * @property $headers array
 */
class HttpResponse
{
    /**
     * @private
     * @var mixed
     */
    private $_body;

    /**
     * @private
     * @var int
     */
    private $_statusCode;

    /**
     * @private
     * @var array
     */
    private $_headers;

    /**
     * HttpResponse constructor.
     * @param $body
     * @param $statusCode
     * @param $headers
     */
    private function __construct($body, $statusCode, $headers)
    {
        $this->_body = $body;
        $this->_statusCode = $statusCode;
        $this->_headers = $headers;
    }

    /**
     * @param $response ResponseInterface
     * @return HttpResponse
     */
    public static function getInstance($response)
    {
        return new HttpResponse(
            $response->getBody(),
            $response->getStatusCode(),
            $response->getHeaders()
        );
    }

    /**
     * @return mixed
     */
    public function getBody()
    {
        return $this->_body;
    }

    /**
     * @return mixed
     */
    public function getBodyAsJson()
    {
        return Util::decodeJson($this->body);
    }


    /**
     * @param mixed $body
     */
    public function setBody($body)
    {
        $this->_body = $body;
    }

    /**
     * @return int
     */
    public function getStatusCode()
    {
        return $this->_statusCode;
    }

    /**
     * @param int $statusCode
     */
    public function setStatusCode($statusCode)
    {
        $this->_statusCode = $statusCode;
    }

    /**
     * @return array
     */
    public function getHeaders()
    {
        return $this->_headers;
    }

    /**
     * @param $key
     * @return string
     */
    public function getHeader($key)
    {
        if (isset($this->_headers[$key])) {
            return $this->_headers[$key];
        } else {
            if (isset($this->_headers[strtolower($key)])) {
                return $this->_headers[strtolower($key)];
            }
        }
        return null;
    }

    /**
     * @param array $headers
     */
    public function setHeaders($headers)
    {
        $this->_headers = $headers;
    }

    public function __get($prop)
    {
        switch ($prop) {
            case 'body':
                return $this->getBody();
            case 'statusCode':
                return $this->getStatusCode();
            case 'headers':
                return $this->getHeaders();
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $prop);
                return null;
        }
    }

    public function __isset($prop)
    {
        switch ($prop) {
            case 'body':
                return isset($this->_body);
            case 'statusCode':
                return isset($this->_statusCode);
            case 'headers':
                return isset($this->_headers);
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $prop);
                return false;
        }
    }

    public function __set($prop, $value)
    {
        switch ($prop) {
            case 'body':
                $this->setBody($value);
                break;
            case 'statusCode':
                $this->setStatusCode($value);
                break;
            case 'headers':
                $this->setHeaders($value);
                break;
            default:
                trigger_error('Undefined property: ' . __CLASS__ . '::$' . $prop);
        }
    }
}