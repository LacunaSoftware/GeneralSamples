<?php


namespace Lacuna\RestPki;

/**
 * Class ValidateFileResponse
 * @package Lacuna\RestPki
 *
 * @property string $accept
 * @property string $rejectReason
 */
class ValidateFileResponse
{
    public $accept;
    public $rejectReason;

    public function __construct($accept, $rejectReason = null)
    {
        $this->accept = $accept;
        $this->rejectReason = $rejectReason;
    }
}
