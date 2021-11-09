<?php


namespace Lacuna\RestPki;

/**
 * Class SignatureSessionStatus
 * @package Lacuna\RestPki
 */
class SignatureSessionStatus
{
    const PENDING = 'Pending';
    const PROCESSING = 'Processing';
    const COMPLETED = 'Completed';
    const USER_CANCELLED = 'UserCancelled';
    const EXPIRED = 'Expired';
    const PROCESSING_ERROR = 'ProcessingError';
}
