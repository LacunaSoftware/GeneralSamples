<?php


namespace Lacuna\RestPki;

/**
 * Class CreateApplicationApiKeyResponse
 * @package Lacuna\RestPki
 *
 * @property string $key
 * @property ApplicationKeyModel $info
 */
class CreateApplicationApiKeyResponse
{
    public $key;
    public $info;

    public function __construct($model)
    {
        $this->key = $model->key;
        $this->info = $model->info;
    }
}
