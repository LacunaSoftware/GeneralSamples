<?php


namespace Lacuna\RestPki;

/**
 * Class RootAuthorizationData
 * @package Lacuna\RestPki
 *
 * @property RootRoles $roles
 * @property string $grantAppId
 */
class RootAuthorizationData
{
    public $roles = null;
    public $grantAppId = null;

    public function __construct()
    {
    }
}
