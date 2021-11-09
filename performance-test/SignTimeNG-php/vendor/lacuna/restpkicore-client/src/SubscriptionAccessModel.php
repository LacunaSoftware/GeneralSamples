<?php


namespace Lacuna\RestPki;

/**
 * Class SubscriptionAccessModel
 * @package Lacuna\RestPki
 *
 * @property Roles[] $roles
 * @property string $subscriptionId
 * @property string[] $parentRoles
 */
class SubscriptionAccessModel
{
    public $subscriptionId = null;
    public $roles = null;
    public $parentRoles = null;

    public function __construct($model)
    {
        $this->subscriptionId = $model->subscriptionId;

        $this->parentRoles = array();
        if(isset($model->parentRoles)){
            foreach ($model->parentRoles as $parentRoles){
                array_push($this->parentRoles, $parentRoles);
            }
        }

        $this->roles = array();
        if(isset($model->roles)){
            foreach ($model->roles as $roles){
                array_push($this->roles, $roles);
            }
        }
    }
}
