<?php


namespace Lacuna\RestPki;

/**
 * Class ValidateFileRequest
 * @package Lacuna\RestPki
 *
 * @property string $name
 * @property string $contentType
 * @property int $length
 */
class ValidateFileRequest
{
    public $name;
    public $contentType;
    public $length;

    public function __construct($model)
    {
        $this->name = $model->name;
        $this->contentType = $model->contentType;
        if (isset($model->length)) {
            $this->length = intval($model->length);
        }
    }
}
