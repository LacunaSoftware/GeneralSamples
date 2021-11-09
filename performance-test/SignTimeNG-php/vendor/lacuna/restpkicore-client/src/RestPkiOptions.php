<?php


namespace Lacuna\RestPki;

/**
 * Class RestPkiOptions
 * @package Lacuna\RestPki
 *
 * @property $endpoint
 * @property $apiKey
 * @property $cultureName
 * @property $usePhpCAInfo
 * @property $caInfoPath
 */
class RestPkiOptions
{
    public $endpoint;
    public $apiKey;
    public $cultureName;
    public $usePhpCAInfo;
    public $caInfoPath;

    public function __construct(
        $endpoint,
        $apiKey = null,
        $cultureName = null,
        $usePhpCAInfo = false,
        $caInfoPath = null
    ) {
        $this->endpoint = $endpoint;
        $this->apiKey = $apiKey;
        $this->cultureName = $cultureName;
        $this->usePhpCAInfo = $usePhpCAInfo;

        if (!isset($caInfoPath)) {
            $caInfoPath = __DIR__ . '/../resources/ca-bundle.pem';
        }
        $this->caInfoPath = $caInfoPath;
    }
}