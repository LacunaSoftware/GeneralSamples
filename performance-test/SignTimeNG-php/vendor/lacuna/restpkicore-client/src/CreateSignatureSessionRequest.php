<?php


namespace Lacuna\RestPki;

/**
 * Class CreateSignatureSessionRequest
 * @package Lacuna\RestPki
 *
 * @property string $returnUrl
 * @property string $securityContextId
 * @property string $callbackArgument
 * @property bool $enableBackgroundProcessing
 * @property bool $disableDownloads
 * @property array $documentMetadata
 */
class CreateSignatureSessionRequest
{
    public $returnUrl;
    public $securityContextId;
    public $callbackArgument;
    public $enableBackgroundProcessing;
    public $disableDownloads;
    public $documentMetadata;

    public function __construct(
        $returnUrl = null,
        $securityContextId = null,
        $callbackArgument = null,
        $enableBackgroundProcessing = false,
        $disableDownloads = false,
        $documentMetadata = null
        ) {
        $this->returnUrl = $returnUrl;
        $this->securityContextId = $securityContextId;
        $this->callbackArgument = $callbackArgument;
        $this->enableBackgroundProcessing = $enableBackgroundProcessing;
        $this->disableDownloads = $disableDownloads;
        $this->documentMetadata = $documentMetadata;
    }
}