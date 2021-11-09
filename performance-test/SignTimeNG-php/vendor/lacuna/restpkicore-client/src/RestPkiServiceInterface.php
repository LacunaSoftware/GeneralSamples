<?php


namespace Lacuna\RestPki;

/**
 * interface RestPkiServiceInterface
 * @package Lacuna\RestPki
 */
interface RestPkiServiceInterface
{
    // Signature Session
    public function getSignatureSession($sessionId);

    public function createSignatureSession($sessionRequest, $subscriptionId = null);

    // Documents
    public function getDocument($documentId);

    public function findDocumentByKey($key);

    public function getDocumentSigners($documentId);

    public function openRead($downloadLink);

    public function getContent($downloadLink);

    public function allocateDocumentKey($provisionalMetadata, $subscriptionId);

    public function allocateDocumentKeys($count, $provisionalMetadata, $subscriptionId);

    // region Application management

    public function createApplication($name, $roles,  $defaultDocumentMetadata, $subscriptionId);

    public function createApplicationKey($applicationId, $expiresOn, $description);

    public function createApplicationAndKey($name, $roles,  $defaultDocumentMetadata, $subscriptionId);

    public function getApplicationDefaultDocumentMetadata($applicationId);

    public function updateApplicationDefaultDocumentMetadata($applicationId,  $defaultDocumentMetadata);

    // endregion
}