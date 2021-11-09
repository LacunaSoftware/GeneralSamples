<?php


namespace Lacuna\RestPki;

/**
 * Class DocumentKeyModel
 * @package Lacuna\RestPki
 *
 * @property string $key
 * @property string $formattedKey
 */
class DocumentKeyModel
{
    public $key;
    public $formattedKey;

    public function __construct($model)
    {
        $this->key = $model->key;
        $this->formattedKey = $model->formattedKey;
    }
}
