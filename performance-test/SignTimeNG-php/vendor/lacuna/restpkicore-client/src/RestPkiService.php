<?php


namespace Lacuna\RestPki;

use Psr\Http\Message\StreamInterface;
use DateTime;

/**
 * Class RestPkiService
 * @package Lacuna\RestPki
 */
class RestPkiService implements RestPkiServiceInterface
{
    protected $_client;

    /**
     * RestPkiService constructor.
     * @param RestPkiCoreClient $client
     */
    public function __construct($client)
    {
        $this->_client = $client;
    }

    /**
     * @param string $documentId
     * @return Document
     */
    public function getDocument($documentId)
    {
        if (empty($documentId)) {
            throw new \Exception("The document ID was not given.");
        }
        $client = $this->_client->getRestClient();
        $response = $client->get(ApiRoutes::DOCUMENTS . '/' . $documentId);
        return new Document($response->getBodyAsJson());
    }

    /**
     * @param string $key
     * @return Document
     */
    public function findDocumentByKey($key)
    {
        if (empty($key)) {
            throw new \Exception("The document key was not given.");
        }
        $client = $this->_client->getRestClient();
        $response = $client->get(ApiRoutes::DOCUMENTS . '/keys/' . $key);
        $model = $response->getBodyAsJson();
        if($model->found && isset($model->document)){
            return new Document($model->document);
        } else {
            return null;
        }
    }

    /**
     * @param string $documentId
     * @return Signer[]
     */
    public function getDocumentSigners($documentId)
    {
        if (empty($documentId)) {
            throw new \Exception("The document ID was not given.");
        }
        $client = $this->_client->getRestClient();
        $response = $client->get(ApiRoutes::DOCUMENTS . '/' . $documentId . "/signers");
        $model = $response->getBodyAsJson();
        $signers = array();
        foreach ($model as $signerModel) {
            array_push($signers, new Signer($signerModel));
        }
        return $signers;
    }

    /**
     * @param string $downloadLink
     * @return StreamInterface
     */
    public function openRead($downloadLink)
    {
        if (empty($downloadLink)) {
            throw new \Exception("The download link was not given.");
        }
        $client = $this->_client->getRestClient();
        return $client->openStream($downloadLink);
    }

    /**
     * @param string $downloadLink
     * @return string|false
     */
    public function getContent($downloadLink)
    {
        if (empty($downloadLink)) {
            throw new \Exception("The download link was not given.");
        }
        $stream = $this->openRead($downloadLink);
        $content = $stream->getContents();
        $stream->close();
        return $content;
    }

    /**
     * @param string $sessionId
     * @return SignatureSession
     */
    public function getSignatureSession($sessionId)
    {
        if (empty($sessionId)) {
            throw new \Exception("The session ID was not given.");
        }
        $client = $this->_client->getRestClient();
        $response = $client->get(ApiRoutes::SIGNATURE_SESSION . '/' . $sessionId);
        return new SignatureSession($response->getBodyAsJson());
    }

    /**
     * @param CreateSignatureSessionRequest $sessionRequest
     * @param string $subscriptionId
     * @return CreateSignatureSessionResponse
     */
    public function createSignatureSession($sessionRequest, $subscriptionId = null)
    {
        if (empty($sessionRequest->returnUrl) && !$sessionRequest->enableBackgroundProcessing) {
            throw new \Exception("The return URL was not given and the Background Processing is not enabled.");
        }
        $customHeaders = [];
        if (isset($subscriptionId)) {
            $customHeaders['X-Subscription'] = $subscriptionId;
        }
        $client = $this->_client->getRestClient($customHeaders);
        $response = $client->post(ApiRoutes::SIGNATURE_SESSION . '/', $sessionRequest);
        return new CreateSignatureSessionResponse($response->getBodyAsJson());
    }

    /**
     * @param array $provisionalMetadata
     * @param string $subscriptionId
     * @return DocumentKeyModel
     */
    public function allocateDocumentKey($provisionalMetadata = null, $subscriptionId = null){
        $customHeaders = [];
        if (isset($subscriptionId)) {
            $customHeaders['X-Subscription'] = $subscriptionId;
        }
        $client = $this->_client->getRestClient($customHeaders);

        $request = new AllocateDocumentKeyRequest($provisionalMetadata);

        $response = $client->post(ApiRoutes::DOCUMENT_KEYS . '/', $request);
        return new DocumentKeyModel($response->getBodyAsJson());
    }

    /**
     * @param int $count
     * @param array $provisionalMetadata
     * @param string $subscriptionId
     * @return DocumentKeyModel[]
     */
    public function allocateDocumentKeys($count, $provisionalMetadata = null, $subscriptionId = null){
        $customHeaders = [];
        if (isset($subscriptionId)) {
            $customHeaders['X-Subscription'] = $subscriptionId;
        }
        $client = $this->_client->getRestClient($customHeaders);

        $request = new AllocateDocumentKeyBatchRequest($count, $provisionalMetadata);

        $response = $client->post(ApiRoutes::DOCUMENT_KEYS . '/batch/', $request);

        $documentKeys = [];
        foreach ($response->getBodyAsJson() as $documentKey) {
            array_push($documentKeys, new DocumentKeyModel($documentKey));
        }
        return $documentKeys;
    }

    /**
     * @param string $name
     * @param Roles[] $roles
     * @param array $defaultDocumentMetadata
     * @param string $subscriptionId
     * @return ApplicationModel
     */
    public function createApplication($name, $roles,  $defaultDocumentMetadata = null, $subscriptionId = null){
        $customHeaders = [];
        if (isset($subscriptionId)) {
            $customHeaders['X-Subscription'] = $subscriptionId;
        }
        $client = $this->_client->getRestClient($customHeaders);

        $authData = new AuthorizationData($roles);
        $rootAuthData = new RootAuthorizationData();
        $request = new ApplicationData();
        $request->name = $name;
        $request->authorizationData = $authData;
        $request->rootAuthorizationData = $rootAuthData;

        $response = $client->post(ApiRoutes::APPLICATIONS . '/', $request);
        $applicationModel = new ApplicationModel($response->getBodyAsJson());

        if($defaultDocumentMetadata != null && !empty($defaultDocumentMetadata)) {
            $this->updateApplicationDefaultDocumentMetadata($applicationModel->id, $defaultDocumentMetadata);
        }

        return $applicationModel;
    }

    /**
     * @param string $applicationId
     * @param DateTime $expiresOn
     * @param string $description
     * @return CreateApplicationApiKeyResponse
     */
    public function createApplicationKey($applicationId, $expiresOn = null, $description = null){
        if ($description == null || empty($description)){
            $description = "Generated on " . (new DateTime())->format('Y-m-d H:i:sp');
        }

        $request = new CreateApplicationApiKeyRequest();
        $request->description = $description;
        $request->expiresOn = $expiresOn;

        $client = $this->_client->getRestClient();
        $response = $client->post(ApiRoutes::APPLICATIONS . "/" . $applicationId . "/api-keys/", $request);
        return new CreateApplicationApiKeyResponse($response->getBodyAsJson());
    }

    /**
     * @param string $name
     * @param Roles[] $roles
     * @param array $defaultDocumentMetadata
     * @param string $subscriptionId
     * @return array
     */
    public function createApplicationAndKey($name, $roles,  $defaultDocumentMetadata = null, $subscriptionId = null){
        $app = $this->createApplication($name, $roles, $defaultDocumentMetadata, $subscriptionId);
        $key = $this->createApplicationKey($app->id);
        return [$app, $key->key];
    }

    /**
     * @param string $applicationId
     * @return array
     */
    public function getApplicationDefaultDocumentMetadata($applicationId){
        $client = $this->_client->getRestClient();
        $response = $client->get(ApiRoutes::APPLICATIONS . "/" . $applicationId . "/default-document-metadata/", $request);
        return $response->getBodyAsJson();
    }

    /**
     * @param string $applicationId
     * @param array $defaultDocumentMetadata
     * @return array
     */
    public function updateApplicationDefaultDocumentMetadata($applicationId,  $defaultDocumentMetadata){
        $client = $this->_client->getRestClient();
        $response = $client->put(ApiRoutes::APPLICATIONS . "/" . $applicationId . "/default-document-metadata/", $defaultDocumentMetadata);
        return $response->getBodyAsJson();
    }
}