using UserCetrificateAutoEnrollment.BL.Windows;

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


        public static Dictionary<SSTTypesEnum, string> SSTTypes = new Dictionary<SSTTypesEnum, string>
        {
            {SSTTypesEnum.AUTHROOTS          , "https://rootca.allianz.com/trust/authroots.sst"},
            {SSTTypesEnum.DISALLOWEDCERT     , "https://rootca.allianz.com/trust/disallowedcert.sst"},
            {SSTTypesEnum.DELDISALLOWED      , "https://rootca.allianz.com/trust/deldisallowed.sst"},
            {SSTTypesEnum.INTERCEPTION       , "https://rootca.allianz.com/trust/interception.sst"},
            {SSTTypesEnum.ROOTS              , "https://rootca.allianz.com/trust/roots.sst"},
            {SSTTypesEnum.DELROOTS           , "https://rootca.allianz.com/trust/delroots.sst"},
            {SSTTypesEnum.UPDROOTS           , "https://rootca.allianz.com/trust/updroots.sst"},
            {SSTTypesEnum.TRUSTEDPUBLISHER   , "https://rootca.allianz.com/trust/trustedpublisher.sst"},
            {SSTTypesEnum.UNTRUSTEDPUBLISHER , "https://rootca.allianz.com/trust/untrustedpublisher.sst"}
        };
    }
}
