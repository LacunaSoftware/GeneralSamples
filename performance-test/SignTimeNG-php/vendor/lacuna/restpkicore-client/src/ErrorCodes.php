<?php


namespace Lacuna\RestPki;

/**
 * Class ErrorCodes
 * @package Lacuna\RestPki
 */
class ErrorCodes
{
    const DOCUMENT_NOT_FOUND = 'DocumentNotFound';
    const SECURITY_CONTEXT_NOT_FOUND = 'SecurityContextNotFound';
    const SIGNATURE_SESSION_NOT_FOUND = 'SignatureSessionNotFound';
    const BAD_SIGNATURE_SESSION_OPERATION = 'BadSignatureSessionOperation';
    const BACKGROUND_PROCESSING = 'BackgroundProcessing';
    const SIGNATURE_SESSION_TOKEN_REQUIRED = 'SignatureSessionTokenRequired';
    const BAD_SIGNATURE_SESSION_TOKEN = 'BadSignatureSessionToken';
    const EXPIRED_SIGNATURE_SESSION_TOKEN = 'ExpiredSignatureSessionToken';
}