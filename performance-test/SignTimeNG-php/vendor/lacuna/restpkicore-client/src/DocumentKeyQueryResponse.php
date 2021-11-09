<?php


namespace Lacuna\RestPki;

/**
 * Class DocumentKeyQueryResponse
 * @package Lacuna\RestPki
 *
 * @property bool $found
 * @property string $formattedKey
 * @property bool $hasDocument
 * @property array $provisionalMetadata
 * @property DocumentModel $document
 */
class DocumentKeyQueryResponse
{
    public $found;
    public $formattedKey;
    public $hasDocument;
    public $provisionalMetadata;
    public $document;

    public function __construct($model)
    {
        $this->found = $model->found;
        $this->formattedKey = $model->formattedKey;
        $this->hasDocument = $model->hasDocument;
        $this->provisionalMetadata = $model->provisionalMetadata;

        if(isset($model->document)){
            $this->document = new DocumentModel($model->document);
        }
    }
}
