namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public static class Constants
    {
        public const string CONTAINER_NAME = "SebiCCLaptopMetafinantzContainer";

        public const string CERTIFICATE_NAME = "CertificateWithProperty";

        public const string STRING_TO_ENCODE = "This is a very very big secret";
        public const string RSA_KEY_HEADER = "-----BEGIN RSA PRIVATE KEY-----";
        public const string RSA_KEY_FOOTER = "-----END RSA PRIVATE KEY-----";
        private const string PublicKeyHeader = "-----BEGIN PUBLIC KEY-----";
        private const string PublicKeyFooter = "-----END PUBLIC KEY-----";
        private const string PublicCertificateHeader = "-----BEGIN CERTIFICATE-----";
        private const string PublicCertificateFooter = "-----END CERTIFICATE-----";


        public const string CertificateNameStart = "CN=";


        public const string ALLIANZ_CERTIFICATE_SUBJECT = "CN=AZ Tech PKI Trust Signing, OU=TrustManager, O=Allianz, C=DE";
    }
}
