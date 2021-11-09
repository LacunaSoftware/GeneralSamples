<?php


namespace Lacuna\RestPki;

/**
 * Class AllocateDocumentKeyBatchRequest
 * @package Lacuna\RestPki
 *
 * @property int $count
 * @property array $provisionalMetadata
 */
class AllocateDocumentKeyBatchRequest
{
    public $count;
    public $provisionalMetadata;

    public function __construct($count = 0, $provisionalMetadata = null)
    {
        $this->count = $count;
        $this->provisionalMetadata = $provisionalMetadata;
    }
}
