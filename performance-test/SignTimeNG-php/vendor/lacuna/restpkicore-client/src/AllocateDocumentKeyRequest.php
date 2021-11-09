<?php


namespace Lacuna\RestPki;

/**
 * Class AllocateDocumentKeyRequest
 * @package Lacuna\RestPki
 *
 * @property array $provisionalMetadata
 */
class AllocateDocumentKeyRequest
{
    public $provisionalMetadata;

    public function __construct($provisionalMetadata = null)
    {
        $this->provisionalMetadata = $provisionalMetadata;
    }
}
