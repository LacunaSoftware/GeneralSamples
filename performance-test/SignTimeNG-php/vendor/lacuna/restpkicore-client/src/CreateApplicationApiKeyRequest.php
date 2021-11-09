<?php


namespace Lacuna\RestPki;

/**
 * Class CreateApplicationApiKeyRequest
 * @package Lacuna\RestPki
 *
 * @property string $description
 * @property DateTime $expiresOn
 */
class CreateApplicationApiKeyRequest
{

    public $description;
    public $expiresOn;

    public function __construct()
    {
    }
}
