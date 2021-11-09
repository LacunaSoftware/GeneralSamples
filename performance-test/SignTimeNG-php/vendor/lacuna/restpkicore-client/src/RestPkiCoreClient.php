<?php


namespace Lacuna\RestPki;

/**
 * Class RestPkiCoreClient
 * @package Lacuna\RestPki
 */
class RestPkiCoreClient
{
    protected $_restClient;
    private $options;

    /**
     * RestPkiCoreClient constructor.
     * @param RestPkiOptions $options
     */
    public function __construct($options)
    {
        $this->options = $options;
    }

    /**
     * @public
     *
     * Gets an client to perform the HTTP requests.
     *
     * @param $customRequestHeaders array
     * @return RestClient The REST client used to perform the HTTP requests.
     */
    public function getRestClient($customRequestHeaders = [])
    {
        if (!isset($this->_restClient)) {
            $this->_restClient = new RestClient(
                $this->options->endpoint,
                $this->options->apiKey,
                $this->options->usePhpCAInfo,
                $this->options->caInfoPath,
                $this->options->cultureName,
                $customRequestHeaders
            );
        }
        if (!empty($customRequestHeaders)) {
            $this->_restClient->customRequestHeaders = $customRequestHeaders;
        }
        return $this->_restClient;
    }
}
