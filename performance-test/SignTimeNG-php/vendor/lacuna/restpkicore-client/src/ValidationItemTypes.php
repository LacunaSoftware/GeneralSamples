<?php


namespace Lacuna\RestPki;

/**
 * Class ValidationItemTypes
 * @package Lacuna\RestPki
 */
class ValidationItemTypes
{
    const SUCCESS = 'Success';
    const CERTIFICATE_NOT_YET_VALID = 'CertificateNotYetValid';
    const CERTIFICATE_EXPIRED = 'CertificateExpired';
    const CERTIFICATE_REVOKED = 'CertificateRevoked';
    const CERTIFICATE_ISSUER_NOT_FOUND = 'CertificateIssuerNotFound';
    const CERTIFICATE_REVOCATION_STATUS_UNKNOWN = 'CertificateRevocationStatusUnknown';
    const CERTIFICATE_CHAIN_ROOT_NOT_TRUSTED = 'CertificateChainRootNotTrusted';
    const INVALID_CERTIFICATE_SIGNATURE = 'InvalidCertificateSignature';
    const DECODE_ERROR = 'DecodeError';
    const REQUIRED_SIGNED_ATTRIBUTE_MISSING = 'RequiredSignedAttributeMissing';
    const FORBIDDEN_SIGNED_ATTRIBUTE_PRESENT = 'ForbiddenSignedAttributePresent';
    const REQUIRED_UNSIGNED_ATTRIBUTE_MISSING = 'RequiredUnsignedAttributeMissing';
    const FORBIDDEN_UNSIGNED_ATTRIBUTE_PRESENT = 'ForbiddenUnsignedAttributePresent';
    const CONTENT_TYPE_MISMATCH = 'ContentTypeMismatch';
    const MESSAGE_DIGEST_MISMATCH = 'MessageDigestMismatch';
    const SIGNING_CERTIFICATE_DIGEST_MISMATCH = 'SigningCertificateDigestMismatch';
    const SIGNATURE_ALGORITHM_VALIDATION_FAILED = 'SignatureAlgorithmValidationFailed';
    const REVOCATION_DATA_ISSUED_BEFORE_GRACE_PERIOD = 'RevocationDataIssuedBeforeGracePeriod';
    const UNCERTIFIED_DATE_REFERENCE = 'UncertifiedDateReference';
    const SIGNATURE_POLICY_MISMATCH = 'SignaturePolicyMismatch';
    const SIGNING_TIME_OUT_OF_CERTIFICATE_VALIDITY = 'SigningTimeOutOfCertificateValidity';
    const UNKNOWN_SIGNED_ATTRIBUTES_PRESENT = 'UnknownSignedAttributesPresent';
    const UNKNOWN_UNSIGNED_ATTRIBUTES_PRESENT = 'UnknownUnsignedAttributesPresent';
    const TIMESTAMP_WITH_MORE_THAN_ONE_SIGNER = 'TimestampWithMoreThanOneSigner';
    const TIMESTAMP_MESSAGE_IMPRINT_MISMATCH = 'TimestampMessageImprintMismatch';
    const TIMESTAMP_VALIDATION_EXCEPTION = 'TimestampValidationException';
    const COMPLETE_REFERENCES_MISMATCH = 'CompleteReferencesMismatch';
    const INVALID_SIGNATURE_TIMESTAMP = 'InvalidSignatureTimestamp';
    const INVALID_REFERENCES_TIMESTAMP = 'InvalidReferencesTimestamp';
    const INVALID_ARCHIVE_TIMESTAMP = 'InvalidArchiveTimestamp';
    const INVALID_KEY_USAGE = 'InvalidKeyUsage';
    const INVALID_OCSP_RESPONSE = 'InvalidOcspResponse';
    const UNAUTHORIZED_ISSUER = 'UnauthorizedIssuer';
    const UNKNOWN_ROOT_TRUST_STATUS = 'UnknownRootTrustStatus';
    const INVALID_TSL = 'InvalidTsl';
    const INVALID_CRL = 'InvalidCrl';
    const CERTIFICATE_ISSUER_VALID = 'CertificateIssuerValid';
    const CERTIFICATE_ISSUER_INVALID = 'CertificateIssuerInvalid';
    const CERTIFICATE_VALIDATION_FAILED = 'CertificateValidationFailed';
    const SIGNATURE_VULNERABLE_TO_SIGNER_SUBSTITUTION = 'SignatureVulnerableToSignerSubstitution';
    const INVALID_XML_SIGNATURE_SCHEMA = 'InvalidXmlSignatureSchema';
    const XML_D_SIG_CORE_VALIDATION_FAILED = 'XmlDSigCoreValidationFailed';
    const SIGNATURE_TIMESTAMP_IGNORED = 'SignatureTimestampIgnored';
    const INVALID_CERTIFICATION_PATH_LEN = 'InvalidCertificationPathLen';
    const SIGNING_CERTIFICATE_NOT_FOUND = 'SigningCertificateNotFound';
    const UNAUTHORIZED_AC_ISSUER = 'UnauthorizedACIssuer';
    const ALGORITHM_NOT_ALLOWED = 'AlgorithmNotAllowed';
    const UNACCEPTABLE_SIGNATURE_POLICY = 'UnacceptableSignaturePolicy';
}