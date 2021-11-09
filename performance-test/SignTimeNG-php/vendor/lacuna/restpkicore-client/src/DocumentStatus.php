<?php


namespace Lacuna\RestPki;

/**
 * Class DocumentStatus
 * @package Lacuna\RestPki
 */
class DocumentStatus
{
    const PENDING_SIGNATURE = 'PendingSignature';
    const PROCESSING = 'Processing';
    const COMPLETED = 'Completed';
    const PROCESSING_ERROR = 'ProcessingError';
}
