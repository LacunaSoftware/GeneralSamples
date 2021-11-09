<?php


namespace Lacuna\RestPki;

/**
 * Class ApplicationModelPaginatedSearchResponse
 * @package Lacuna\RestPki
 *
 * @property int $totalCount
 * @property ApplicationModel[] $items
 */
class ApplicationModelPaginatedSearchResponse
{
    public $totalCount;
    public $items;

    public function __construct($model)
    {
        $this->totalCount = $model->totalCount;

        $this->items = array();
        if(isset($model->items)){
            foreach ($model->items as $items){
                array_push($this->items, new ApplicationModel($items));
            }
        }        
    }
}
