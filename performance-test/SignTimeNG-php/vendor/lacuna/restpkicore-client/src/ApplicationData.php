<?php


namespace Lacuna\RestPki;

/**
 * Class ApplicationData
 * @package Lacuna\RestPki
 *
 * @property string $name
 * @property AuthorizationData $authorizationData
 * @property RootAuthorizationData $rootAuthorizationData
 */
class ApplicationData
{
    public $name;
    public $authorizationData;
    public $rootAuthorizationData;

    public function __construct()
    {
    }
}
