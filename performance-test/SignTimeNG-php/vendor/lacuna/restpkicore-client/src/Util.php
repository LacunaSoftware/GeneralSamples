<?php


namespace Lacuna\RestPki;

/**
 * @internal
 *
 * Class Util
 * @package Lacuna\RestPki
 */
class Util
{
    /**
     * @param $obj
     * @return false|string
     * @internal
     *
     */
    static function encodeJson($obj)
    {
        return json_encode($obj);
    }

    /**
     * @param $json
     * @return mixed
     * @internal
     *
     */
    static function decodeJson($json)
    {
        return json_decode($json);
    }
}