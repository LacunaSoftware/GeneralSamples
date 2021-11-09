<?php


namespace Lacuna\RestPki;

/**
 * Class WebhookEvent
 * @package Lacuna\RestPki
 *
 * @property WebhookEventTypes $type
 * @property Document $document
 */
class WebhookEvent
{
    public $type;
    public $document;

    public function __construct($model)
    {
        $this->type = $model->type;
        if (isset($model->document)) {
            $this->document = new Document($model->document);
        }
    }
}
