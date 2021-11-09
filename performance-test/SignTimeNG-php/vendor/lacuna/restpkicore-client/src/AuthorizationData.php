<?php


namespace Lacuna\RestPki;

/**
 * Class AuthorizationData
 * @package Lacuna\RestPki
 *
 * @property Roles $roles
 */
class AuthorizationData
{
    public $roles;

    public function __construct($roles = null)
    {
        $this->roles = $roles;
    }
}
