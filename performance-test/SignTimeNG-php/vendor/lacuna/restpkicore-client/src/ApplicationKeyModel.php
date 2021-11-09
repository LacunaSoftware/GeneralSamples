<?php


namespace Lacuna\RestPki;
use DateTime;

/**
 * Class ApplicationKeyModel
 * @package Lacuna\RestPki
 *
 * @property string $id
 * @property ApplicationKeyTypes $type
 * @property string $applicationId
 * @property string $description
 * @property DateTime $expiresOn
 * @property string $certificateThumbprint
 */
class ApplicationKeyModel
{
    public $id;
    public $type;
    public $applicationId;
    public $description;
    public $expiresOn;
    public $certificateThumbprint;

    public function __construct($model)
    {
        $this->id = $model->id;
        $this->applicationId = $model->applicationId;
        $this->description = $model->description;
        $this->certificateThumbprint = $model->certificateThumbprint;

        if(isset($model->expiresOn)){
            $this->expiresOn = new DateTime($model->expiresOn);
        }

        $this->type = $model->type;
    }
}
