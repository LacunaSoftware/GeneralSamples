<?php


namespace Lacuna\RestPki;

/**
 * Class ApplicationModel
 * @package Lacuna\RestPki
 *
 * @property string $id
 * @property string $subscriptionId
 * @property string $name
 * @property RootRoles[] $rootRoles
 * @property SubscriptionAccessModel[] $subscriptions
 * @property string $grantAppId
 * 
 */
class ApplicationModel
{
    public $id;
    public $subscriptionId;
    public $name;
    public $rootRoles;
    public $subscriptions;
    public $grantAppId;

    public function __construct($model)
    {
        $this->id = $model->id;
        $this->subscriptionId = $model->subscriptionId;
        $this->name = $model->name;
        $this->grantAppId = $model->grantAppId;

        $this->rootRoles = array();
        if(isset($model->rootRoles)){
            foreach ($model->rootRoles as $rootRoles){
                array_push($this->rootRoles, $rootRoles);
            }
        }
        
        $this->subscriptions = array();
        if(isset($model->subscriptions)){
            foreach ($model->subscriptions as $subscriptions){
                array_push($this->subscriptions, new SubscriptionAccessModel($subscriptions));
            }
        }
    }
}
