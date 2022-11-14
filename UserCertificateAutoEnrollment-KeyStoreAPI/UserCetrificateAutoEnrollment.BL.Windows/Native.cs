using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UserCetrificateAutoEnrollment.BL.Windows
{
    public static class Native
    {
        #region PInvoke Native Declarations
        private const string KEY_CONTAINER_NAME = "UASDKDefaultKeyContainer3";
        private const string MS_STRONG_PROV_W = "Microsoft Strong Cryptographic Provider";
        private const string DEFAULT_CRYPTO_PROVIDER = MS_STRONG_PROV_W;
        private const int X509_ASN_ENCODING = 0x00000001;
        private const int PKCS_7_ASN_ENCODING = 0x00010000;
        private const int CRYPT_DECODE_ALLOC_FLAG = 0x8000;
        private const int CRYPT_DECODE_NOCOPY_FLAG = 0x1;

        private const int AT_KEYEXCHANGE = 1;
        private const int AT_SIGNATURE = 2;

        private const int REPORT_NO_PRIVATE_KEY = 0x0001;
        private const int REPORT_NOT_ABLE_TO_EXPORT_PRIVATE_KEY = 0x0002;
        private const int EXPORT_PRIVATE_KEYS = 0x0004;

        private const int CERT_SET_KEY_PROV_HANDLE_PROP_ID = 0x00000001;
        private const int CERT_SET_KEY_CONTEXT_PROP_ID = 0x00000001;

        //+-------------------------------------------------------------------------
        //  Certificate name string types
        //--------------------------------------------------------------------------
        private const int CERT_SIMPLE_NAME_STR = 1;
        private const int CERT_OID_NAME_STR = 2;
        private const int CERT_X500_NAME_STR = 3;

        //+-------------------------------------------------------------------------
        //  Certificate name string type flags OR'ed with the above types
        //--------------------------------------------------------------------------
        private const int CERT_NAME_STR_SEMICOLON_FLAG = 0x40000000;
        private const int CERT_NAME_STR_NO_PLUS_FLAG = 0x20000000;
        private const int CERT_NAME_STR_NO_QUOTING_FLAG = 0x10000000;
        private const int CERT_NAME_STR_CRLF_FLAG = 0x08000000;
        private const int CERT_NAME_STR_COMMA_FLAG = 0x04000000;
        private const int CERT_NAME_STR_REVERSE_FLAG = 0x02000000;

        private const int CERT_NAME_STR_DISABLE_IE4_UTF8_FLAG = 0x00010000;
        private const int CERT_NAME_STR_ENABLE_T61_UNICODE_FLAG = 0x00020000;
        private const int CERT_NAME_STR_ENABLE_UTF8_UNICODE_FLAG = 0x00040000;

        // dwFlags definitions for CryptAcquireContext
        private const int CRYPT_VERIFYCONTEXT = unchecked((int)0xF0000000);
        private const int CRYPT_NEWKEYSET = 0x00000008;
        private const int CRYPT_DELETEKEYSET = 0x00000010;
        private const int CRYPT_MACHINE_KEYSET = 0x00000020;
        private const int CRYPT_USER_KEYSET = 0x00001000;
        private const int CRYPT_SILENT = 0x00000040;

        // dwFlag definitions for CryptGenKey
        private const int CRYPT_EXPORTABLE = 0x00000001;
        private const int CRYPT_USER_PROTECTED = 0x00000002;
        private const int CRYPT_CREATE_SALT = 0x00000004;
        private const int CRYPT_UPDATE_KEY = 0x00000008;
        private const int CRYPT_NO_SALT = 0x00000010;
        private const int CRYPT_PREGEN = 0x00000040;
        private const int CRYPT_RECIPIENT = 0x00000010;
        private const int CRYPT_INITIATOR = 0x00000040;
        private const int CRYPT_ONLINE = 0x00000080;
        private const int CRYPT_SF = 0x00000100;
        private const int CRYPT_CREATE_IV = 0x00000200;
        private const int CRYPT_KEK = 0x00000400;
        private const int CRYPT_DATA_KEY = 0x00000800;
        private const int CRYPT_VOLATILE = 0x00001000;
        private const int CRYPT_SGCKEY = 0x00002000;
        private const int CRYPT_ARCHIVABLE = 0x00004000;

        private const int CRYPT_FIRST = 0x00000001;
        private const int CRYPT_NEXT = 0x00000002;
        private const int PP_ENUMCONTAINERS = 0x00000002;
        private const int PP_CONTAINER = 6;
        private const int ERROR_MORE_DATA = 234;
        private const int KP_CERTIFICATE = 26;
        private const int KP_KEYLEN = 9;

        private const int PROV_RSA_FULL = 1;

        private const int NTE_EXISTS = -0x7FF6FFF1; // 0x8009000F
        private const int NTE_BAD_KEYSET = -0x7FF6FFEA; // 0x80090016
        private const int CRYPT_E_NOT_FOUND = -0x7FF6DFFC; // 0x80092004L

        //+-------------------------------------------------------------------------
        //  Certificate Store open/property flags
        //--------------------------------------------------------------------------
        private const int CERT_STORE_NO_CRYPT_RELEASE_FLAG = 0x00000001;
        private const int CERT_STORE_SET_LOCALIZED_NAME_FLAG = 0x00000002;
        private const int CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG = 0x00000004;
        private const int CERT_STORE_DELETE_FLAG = 0x00000010;
        private const int CERT_STORE_UNSAFE_PHYSICAL_FLAG = 0x00000020;
        private const int CERT_STORE_SHARE_STORE_FLAG = 0x00000040;
        private const int CERT_STORE_SHARE_CONTEXT_FLAG = 0x00000080;
        private const int CERT_STORE_MANIFOLD_FLAG = 0x00000100;
        private const int CERT_STORE_ENUM_ARCHIVED_FLAG = 0x00000200;
        private const int CERT_STORE_UPDATE_KEYID_FLAG = 0x00000400;
        private const int CERT_STORE_BACKUP_RESTORE_FLAG = 0x00000800;
        private const int CERT_STORE_READONLY_FLAG = 0x00008000;
        private const int CERT_STORE_OPEN_EXISTING_FLAG = 0x00004000;
        private const int CERT_STORE_CREATE_NEW_FLAG = 0x00002000;
        private const int CERT_STORE_MAXIMUM_ALLOWED_FLAG = 0x00001000;

        // Location of the system store:
        private const int CERT_SYSTEM_STORE_LOCATION_SHIFT = 16;

        //  Registry: HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE
        private const int CERT_SYSTEM_STORE_CURRENT_USER_ID = 1;
        private const int CERT_SYSTEM_STORE_LOCAL_MACHINE_ID = 2;
        //  Registry: HKEY_LOCAL_MACHINE\Software\Microsoft\Cryptography\Services
        private const int CERT_SYSTEM_STORE_CURRENT_SERVICE_ID = 4;
        private const int CERT_SYSTEM_STORE_SERVICES_ID = 5;
        //  Registry: HKEY_USERS
        private const int CERT_SYSTEM_STORE_USERS_ID = 6;

        private const int CERT_SYSTEM_STORE_CURRENT_USER = (CERT_SYSTEM_STORE_CURRENT_USER_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT);
        private const int CERT_SYSTEM_STORE_LOCAL_MACHINE = (CERT_SYSTEM_STORE_LOCAL_MACHINE_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT);
        private const int CERT_SYSTEM_STORE_CURRENT_SERVICE = (CERT_SYSTEM_STORE_CURRENT_SERVICE_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT);
        private const int CERT_SYSTEM_STORE_SERVICES = (CERT_SYSTEM_STORE_SERVICES_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT);
        private const int CERT_SYSTEM_STORE_USERS = (CERT_SYSTEM_STORE_USERS_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT);

        //+--------------------------------"2.5.29.7"-----------------------------------------
        //  Extension Object Identifiers
        //--------------------------------------------------------------------------
        private const string szOID_AUTHORITY_KEY_IDENTIFIER = "2.5.29.1";
        private const string szOID_KEY_ATTRIBUTES = "2.5.29.2";
        private const string szOID_CERT_POLICIES_95 = "2.5.29.3";
        private const string szOID_KEY_USAGE_RESTRICTION = "2.5.29.4";
        // private const string szOID_SUBJECT_ALT_NAME = "2.5.29.7";
        private const string szOID_ISSUER_ALT_NAME = "2.5.29.8";
        private const string szOID_BASIC_CONSTRAINTS = "2.5.29.10";
        private const string szOID_KEY_USAGE = "2.5.29.15";
        private const string szOID_PRIVATEKEY_USAGE_PERIOD = "2.5.29.16";
        private const string szOID_BASIC_CONSTRAINTS2 = "2.5.29.19";

        private const string szOID_CERT_POLICIES = "2.5.29.32";
        private const string szOID_ANY_CERT_POLICY = "2.5.29.32.0";

        private const string szOID_AUTHORITY_KEY_IDENTIFIER2 = "2.5.29.35";
        private const string szOID_SUBJECT_KEY_IDENTIFIER = "2.5.29.14";
        private const string szOID_SUBJECT_ALT_NAME2 = "2.5.29.17";
        private const string szOID_ISSUER_ALT_NAME2 = "2.5.29.18";
        private const string szOID_CRL_REASON_CODE = "2.5.29.21";
        private const string szOID_REASON_CODE_HOLD = "2.5.29.23";
        private const string szOID_CRL_DIST_POINTS = "2.5.29.31";
        private const string szOID_ENHANCED_KEY_USAGE = "2.5.29.37";

        private const string szOID_RSA_RSA = "1.2.840.113549.1.1.1";

        //+-------------------------------------------------------------------------
        //  Enhanced Key Usage (Purpose) Object Identifiers
        //--------------------------------------------------------------------------
        private const string szOID_PKIX_KP = "1.3.6.1.5.5.7.3";

        // Consistent key usage bits: DIGITAL_SIGNATURE, KEY_ENCIPHERMENT
        // or KEY_AGREEMENT
        private const string szOID_PKIX_KP_SERVER_AUTH = "1.3.6.1.5.5.7.3.1";

        // Consistent key usage bits: DIGITAL_SIGNATURE
        private const string szOID_PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";

        // Consistent key usage bits: DIGITAL_SIGNATURE
        private const string szOID_PKIX_KP_CODE_SIGNING = "1.3.6.1.5.5.7.3.3";

        // Consistent key usage bits: DIGITAL_SIGNATURE, NON_REPUDIATION and/or
        // (KEY_ENCIPHERMENT or KEY_AGREEMENT)
        private const string szOID_PKIX_KP_EMAIL_PROTECTION = "1.3.6.1.5.5.7.3.4";

        // Consistent key usage bits: DIGITAL_SIGNATURE and/or
        // (KEY_ENCIPHERMENT or KEY_AGREEMENT)
        private const string szOID_PKIX_KP_IPSEC_END_SYSTEM = "1.3.6.1.5.5.7.3.5";

        // Consistent key usage bits: DIGITAL_SIGNATURE and/or
        // (KEY_ENCIPHERMENT or KEY_AGREEMENT)
        private const string szOID_PKIX_KP_IPSEC_TUNNEL = "1.3.6.1.5.5.7.3.6";

        // Consistent key usage bits: DIGITAL_SIGNATURE and/or
        // (KEY_ENCIPHERMENT or KEY_AGREEMENT)
        private const string szOID_PKIX_KP_IPSEC_USER = "1.3.6.1.5.5.7.3.7";

        // Consistent key usage bits: DIGITAL_SIGNATURE or NON_REPUDIATION
        private const string szOID_PKIX_KP_TIMESTAMP_SIGNING = "1.3.6.1.5.5.7.3.8";

        // IKE (Internet Key Exchange) Intermediate KP for an IPsec end entity.
        // Defined in draft-ietf-ipsec-pki-req-04.txt, December 14, 1999.
        private const string szOID_IPSEC_KP_IKE_INTERMEDIATE = "1.3.6.1.5.5.8.2.2";

        //+-------------------------------------------------------------------------
        //  Predefined X509 certificate extension data structures that can be
        //  encoded / decoded.
        //--------------------------------------------------------------------------
        private const int X509_CERT = 1;
        private const int X509_CERT_CRL_TO_BE_SIGNED = 3;
        private const int X509_AUTHORITY_KEY_ID = 9;
        private const int X509_KEY_ATTRIBUTES = 10;
        private const int X509_KEY_USAGE_RESTRICTION = 11;
        private const int X509_ALTERNATE_NAME = 12;
        private const int X509_BASIC_CONSTRAINTS = 13;
        private const int X509_KEY_USAGE = 14;
        private const int X509_BASIC_CONSTRAINTS2 = 15;
        private const int X509_CERT_POLICIES = 16;

        //+-------------------------------------------------------------------------
        // Certificate comparison functions
        //--------------------------------------------------------------------------
        private const int CERT_COMPARE_MASK = 0xFFFF;
        private const int CERT_COMPARE_SHIFT = 16;
        private const int CERT_COMPARE_ANY = 0;
        private const int CERT_COMPARE_SHA1_HASH = 1;
        private const int CERT_COMPARE_NAME = 2;
        private const int CERT_COMPARE_ATTR = 3;
        private const int CERT_COMPARE_MD5_HASH = 4;
        private const int CERT_COMPARE_PROPERTY = 5;
        private const int CERT_COMPARE_PUBLIC_KEY = 6;
        private const int CERT_COMPARE_HASH = CERT_COMPARE_SHA1_HASH;
        private const int CERT_COMPARE_NAME_STR_A = 7;
        private const int CERT_COMPARE_NAME_STR_W = 8;
        private const int CERT_COMPARE_KEY_SPEC = 9;
        private const int CERT_COMPARE_ENHKEY_USAGE = 10;
        private const int CERT_COMPARE_CTL_USAGE = CERT_COMPARE_ENHKEY_USAGE;
        private const int CERT_COMPARE_SUBJECT_CERT = 11;
        private const int CERT_COMPARE_ISSUER_OF = 12;
        private const int CERT_COMPARE_EXISTING = 13;
        private const int CERT_COMPARE_SIGNATURE_HASH = 14;
        private const int CERT_COMPARE_KEY_IDENTIFIER = 15;
        private const int CERT_COMPARE_CERT_ID = 16;
        private const int CERT_COMPARE_CROSS_CERT_DIST_POINTS = 17;
        private const int CERT_COMPARE_PUBKEY_MD5_HASH = 18;

        //+-------------------------------------------------------------------------
        //  Certificate Information Flags
        //--------------------------------------------------------------------------
        private const int CERT_INFO_VERSION_FLAG = 1;
        private const int CERT_INFO_SERIAL_NUMBER_FLAG = 2;
        private const int CERT_INFO_SIGNATURE_ALGORITHM_FLAG = 3;
        private const int CERT_INFO_ISSUER_FLAG = 4;
        private const int CERT_INFO_NOT_BEFORE_FLAG = 5;
        private const int CERT_INFO_NOT_AFTER_FLAG = 6;
        private const int CERT_INFO_SUBJECT_FLAG = 7;
        private const int CERT_INFO_SUBJECT_PUBLIC_KEY_INFO_FLAG = 8;
        private const int CERT_INFO_ISSUER_UNIQUE_ID_FLAG = 9;
        private const int CERT_INFO_SUBJECT_UNIQUE_ID_FLAG = 10;
        private const int CERT_INFO_EXTENSION_FLAG = 11;

        //+-------------------------------------------------------------------------
        //  dwFindType
        //
        //  The dwFindType definition consists of two components:
        //   - comparison function
        //   - certificate information flag
        //--------------------------------------------------------------------------
        private const int CERT_FIND_ANY = (CERT_COMPARE_ANY << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_SHA1_HASH = (CERT_COMPARE_SHA1_HASH << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_MD5_HASH = (CERT_COMPARE_MD5_HASH << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_SIGNATURE_HASH = (CERT_COMPARE_SIGNATURE_HASH << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_KEY_IDENTIFIER = (CERT_COMPARE_KEY_IDENTIFIER << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_HASH = CERT_FIND_SHA1_HASH;
        private const int CERT_FIND_PROPERTY = (CERT_COMPARE_PROPERTY << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_PUBLIC_KEY = (CERT_COMPARE_PUBLIC_KEY << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_SUBJECT_NAME = (CERT_COMPARE_NAME << CERT_COMPARE_SHIFT | CERT_INFO_SUBJECT_FLAG);
        private const int CERT_FIND_SUBJECT_ATTR = (CERT_COMPARE_ATTR << CERT_COMPARE_SHIFT | CERT_INFO_SUBJECT_FLAG);
        private const int CERT_FIND_ISSUER_NAME = (CERT_COMPARE_NAME << CERT_COMPARE_SHIFT | CERT_INFO_ISSUER_FLAG);
        private const int CERT_FIND_ISSUER_ATTR = (CERT_COMPARE_ATTR << CERT_COMPARE_SHIFT | CERT_INFO_ISSUER_FLAG);
        private const int CERT_FIND_SUBJECT_STR_A = (CERT_COMPARE_NAME_STR_A << CERT_COMPARE_SHIFT | CERT_INFO_SUBJECT_FLAG);
        private const int CERT_FIND_SUBJECT_STR_W = (CERT_COMPARE_NAME_STR_W << CERT_COMPARE_SHIFT | CERT_INFO_SUBJECT_FLAG);
        private const int CERT_FIND_SUBJECT_STR = CERT_FIND_SUBJECT_STR_W;
        private const int CERT_FIND_ISSUER_STR_A = (CERT_COMPARE_NAME_STR_A << CERT_COMPARE_SHIFT | CERT_INFO_ISSUER_FLAG);
        private const int CERT_FIND_ISSUER_STR_W = (CERT_COMPARE_NAME_STR_W << CERT_COMPARE_SHIFT | CERT_INFO_ISSUER_FLAG);
        private const int CERT_FIND_ISSUER_STR = CERT_FIND_ISSUER_STR_W;
        private const int CERT_FIND_KEY_SPEC = (CERT_COMPARE_KEY_SPEC << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_ENHKEY_USAGE = (CERT_COMPARE_ENHKEY_USAGE << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_CTL_USAGE = CERT_FIND_ENHKEY_USAGE;
        private const int CERT_FIND_SUBJECT_CERT = (CERT_COMPARE_SUBJECT_CERT << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_ISSUER_OF = (CERT_COMPARE_ISSUER_OF << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_EXISTING = (CERT_COMPARE_EXISTING << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_CERT_ID = (CERT_COMPARE_CERT_ID << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_CROSS_CERT_DIST_POINTS = (CERT_COMPARE_CROSS_CERT_DIST_POINTS << CERT_COMPARE_SHIFT);
        private const int CERT_FIND_PUBKEY_MD5_HASH = (CERT_COMPARE_PUBKEY_MD5_HASH << CERT_COMPARE_SHIFT);

        // Byte[0]
        private const int CERT_DIGITAL_SIGNATURE_KEY_USAGE = 0x80;
        private const int CERT_NON_REPUDIATION_KEY_USAGE = 0x40;
        private const int CERT_KEY_ENCIPHERMENT_KEY_USAGE = 0x20;
        private const int CERT_DATA_ENCIPHERMENT_KEY_USAGE = 0x10;
        private const int CERT_KEY_AGREEMENT_KEY_USAGE = 0x08;
        private const int CERT_KEY_CERT_SIGN_KEY_USAGE = 0x04;
        private const int CERT_OFFLINE_CRL_SIGN_KEY_USAGE = 0x02;
        private const int CERT_CRL_SIGN_KEY_USAGE = 0x02;
        private const int CERT_ENCIPHER_ONLY_KEY_USAGE = 0x01;
        // Byte[1]
        private const int CERT_DECIPHER_ONLY_KEY_USAGE = 0x80;

        // Algorithm classes
        private const int ALG_CLASS_ANY = (0);
        private const int ALG_CLASS_SIGNATURE = (1 << 13);
        private const int ALG_CLASS_MSG_ENCRYPT = (2 << 13);
        private const int ALG_CLASS_DATA_ENCRYPT = (3 << 13);
        private const int ALG_CLASS_HASH = (4 << 13);
        private const int ALG_CLASS_KEY_EXCHANGE = (5 << 13);
        private const int ALG_CLASS_ALL = (7 << 13);

        // Algorithm types
        private const int ALG_TYPE_ANY = (0);
        private const int ALG_TYPE_DSS = (1 << 9);
        private const int ALG_TYPE_RSA = (2 << 9);
        private const int ALG_TYPE_BLOCK = (3 << 9);
        private const int ALG_TYPE_STREAM = (4 << 9);
        private const int ALG_TYPE_DH = (5 << 9);
        private const int ALG_TYPE_SECURECHANNEL = (6 << 9);

        // Generic sub-ids
        private const int ALG_SID_ANY = (0);

        // Some RSA sub-ids
        private const int ALG_SID_RSA_ANY = 0;
        private const int ALG_SID_RSA_PKCS = 1;
        private const int ALG_SID_RSA_MSATWORK = 2;
        private const int ALG_SID_RSA_ENTRUST = 3;
        private const int ALG_SID_RSA_PGP = 4;

        // Some DSS sub-ids
        //
        private const int ALG_SID_DSS_ANY = 0;
        private const int ALG_SID_DSS_PKCS = 1;
        private const int ALG_SID_DSS_DMS = 2;

        // Block cipher sub ids
        // DES sub_ids
        private const int ALG_SID_DES = 1;
        private const int ALG_SID_3DES = 3;
        private const int ALG_SID_DESX = 4;
        private const int ALG_SID_IDEA = 5;
        private const int ALG_SID_CAST = 6;
        private const int ALG_SID_SAFERSK64 = 7;
        private const int ALG_SID_SAFERSK128 = 8;
        private const int ALG_SID_3DES_112 = 9;
        private const int ALG_SID_CYLINK_MEK = 12;
        private const int ALG_SID_RC5 = 13;
        private const int ALG_SID_AES_128 = 14;
        private const int ALG_SID_AES_192 = 15;
        private const int ALG_SID_AES_256 = 16;
        private const int ALG_SID_AES = 17;

        // Fortezza sub-ids
        private const int ALG_SID_SKIPJACK = 10;
        private const int ALG_SID_TEK = 11;

        // KP_MODE
        private const int CRYPT_MODE_CBCI = 6;       // ANSI CBC Interleaved
        private const int CRYPT_MODE_CFBP = 7;       // ANSI CFB Pipelined
        private const int CRYPT_MODE_OFBP = 8;       // ANSI OFB Pipelined
        private const int CRYPT_MODE_CBCOFM = 9;       // ANSI CBC + OF Masking
        private const int CRYPT_MODE_CBCOFMI = 10;      // ANSI CBC + OFM Interleaved

        // RC2 sub-ids
        private const int ALG_SID_RC2 = 2;

        // Stream cipher sub-ids
        private const int ALG_SID_RC4 = 1;
        private const int ALG_SID_SEAL = 2;

        // Diffie-Hellman sub-ids
        private const int ALG_SID_DH_SANDF = 1;
        private const int ALG_SID_DH_EPHEM = 2;
        private const int ALG_SID_AGREED_KEY_ANY = 3;
        private const int ALG_SID_KEA = 4;

        // Hash sub ids
        private const int ALG_SID_MD2 = 1;
        private const int ALG_SID_MD4 = 2;
        private const int ALG_SID_MD5 = 3;
        private const int ALG_SID_SHA = 4;
        private const int ALG_SID_SHA1 = 4;
        private const int ALG_SID_MAC = 5;
        private const int ALG_SID_RIPEMD = 6;
        private const int ALG_SID_RIPEMD160 = 7;
        private const int ALG_SID_SSL3SHAMD5 = 8;
        private const int ALG_SID_HMAC = 9;
        private const int ALG_SID_TLS1PRF = 10;
        private const int ALG_SID_HASH_REPLACE_OWF = 11;
        private const int ALG_SID_SHA_256 = 12;
        private const int ALG_SID_SHA_384 = 13;
        private const int ALG_SID_SHA_512 = 14;

        // secure channel sub ids
        private const int ALG_SID_SSL3_MASTER = 1;
        private const int ALG_SID_SCHANNEL_MASTER_HASH = 2;
        private const int ALG_SID_SCHANNEL_MAC_KEY = 3;
        private const int ALG_SID_PCT1_MASTER = 4;
        private const int ALG_SID_SSL2_MASTER = 5;
        private const int ALG_SID_TLS1_MASTER = 6;
        private const int ALG_SID_SCHANNEL_ENC_KEY = 7;

        // Our silly example sub-id
        private const int ALG_SID_EXAMPLE = 80;

        // algorithm identifier definitions
        private const int CALG_MD2 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD2);
        private const int CALG_MD4 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD4);
        private const int CALG_MD5 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD5);
        private const int CALG_SHA = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA);
        private const int CALG_SHA1 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA1);
        private const int CALG_MAC = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MAC);
        private const int CALG_RSA_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_RSA | ALG_SID_RSA_ANY);
        private const int CALG_DSS_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_DSS | ALG_SID_DSS_ANY);
        private const int CALG_NO_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_ANY | ALG_SID_ANY);
        private const int CALG_RSA_KEYX = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_RSA | ALG_SID_RSA_ANY);
        private const int CALG_DES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_DES);
        private const int CALG_3DES_112 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_3DES_112);
        private const int CALG_3DES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_3DES);
        private const int CALG_DESX = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_DESX);
        private const int CALG_RC2 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_RC2);
        private const int CALG_RC4 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_STREAM | ALG_SID_RC4);
        private const int CALG_SEAL = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_STREAM | ALG_SID_SEAL);
        private const int CALG_DH_SF = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_SANDF);
        private const int CALG_DH_EPHEM = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EPHEM);
        private const int CALG_AGREEDKEY_ANY = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_AGREED_KEY_ANY);
        private const int CALG_KEA_KEYX = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_KEA);
        private const int CALG_HUGHES_MD5 = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_ANY | ALG_SID_MD5);
        private const int CALG_SKIPJACK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_SKIPJACK);
        private const int CALG_TEK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_TEK);
        private const int CALG_CYLINK_MEK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_CYLINK_MEK);
        private const int CALG_SSL3_SHAMD5 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SSL3SHAMD5);
        private const int CALG_SSL3_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SSL3_MASTER);
        private const int CALG_SCHANNEL_MASTER_HASH = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MASTER_HASH);
        private const int CALG_SCHANNEL_MAC_KEY = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MAC_KEY);
        private const int CALG_SCHANNEL_ENC_KEY = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_ENC_KEY);
        private const int CALG_PCT1_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_PCT1_MASTER);
        private const int CALG_SSL2_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SSL2_MASTER);
        private const int CALG_TLS1_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_TLS1_MASTER);
        private const int CALG_RC5 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_RC5);
        private const int CALG_HMAC = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_HMAC);
        private const int CALG_TLS1PRF = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_TLS1PRF);
        private const int CALG_HASH_REPLACE_OWF = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_HASH_REPLACE_OWF);
        private const int CALG_AES_128 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_128);
        private const int CALG_AES_192 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_192);
        private const int CALG_AES_256 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_256);
        private const int CALG_AES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES);
        private const int CALG_SHA_256 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA_256);
        private const int CALG_SHA_384 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA_384);
        private const int CALG_SHA_512 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA_512);

        //+-------------------------------------------------------------------------
        // Add certificate/CRL, encoded, context or element disposition values.
        //--------------------------------------------------------------------------
        private const int CERT_STORE_ADD_NEW = 1;
        private const int CERT_STORE_ADD_USE_EXISTING = 2;
        private const int CERT_STORE_ADD_REPLACE_EXISTING = 3;
        private const int CERT_STORE_ADD_ALWAYS = 4;
        private const int CERT_STORE_ADD_REPLACE_EXISTING_INHERIT_PROPERTIES = 5;
        private const int CERT_STORE_ADD_NEWER = 6;
        private const int CERT_STORE_ADD_NEWER_INHERIT_PROPERTIES = 7;

        //+-------------------------------------------------------------------------
        //  Certificate, CRL and CTL property IDs
        //
        //  See CertSetCertificateContextProperty or CertGetCertificateContextProperty
        //  for usage information.
        //--------------------------------------------------------------------------
        private const int CERT_KEY_PROV_HANDLE_PROP_ID = 1;
        private const int CERT_KEY_PROV_INFO_PROP_ID = 2;
        private const int CERT_SHA1_HASH_PROP_ID = 3;
        private const int CERT_MD5_HASH_PROP_ID = 4;
        private const int CERT_HASH_PROP_ID = CERT_SHA1_HASH_PROP_ID;
        private const int CERT_KEY_CONTEXT_PROP_ID = 5;
        private const int CERT_KEY_SPEC_PROP_ID = 6;
        private const int CERT_IE30_RESERVED_PROP_ID = 7;
        private const int CERT_PUBKEY_HASH_RESERVED_PROP_ID = 8;
        private const int CERT_ENHKEY_USAGE_PROP_ID = 9;
        private const int CERT_CTL_USAGE_PROP_ID = CERT_ENHKEY_USAGE_PROP_ID;
        private const int CERT_NEXT_UPDATE_LOCATION_PROP_ID = 10;
        private const int CERT_FRIENDLY_NAME_PROP_ID = 11;
        private const int CERT_PVK_FILE_PROP_ID = 12;
        private const int CERT_DESCRIPTION_PROP_ID = 13;
        private const int CERT_ACCESS_STATE_PROP_ID = 14;
        private const int CERT_SIGNATURE_HASH_PROP_ID = 15;
        private const int CERT_SMART_CARD_DATA_PROP_ID = 16;
        private const int CERT_EFS_PROP_ID = 17;
        private const int CERT_FORTEZZA_DATA_PROP_ID = 18;
        private const int CERT_ARCHIVED_PROP_ID = 19;
        private const int CERT_KEY_IDENTIFIER_PROP_ID = 20;
        private const int CERT_AUTO_ENROLL_PROP_ID = 21;
        private const int CERT_PUBKEY_ALG_PARA_PROP_ID = 22;
        private const int CERT_CROSS_CERT_DIST_POINTS_PROP_ID = 23;
        private const int CERT_ISSUER_PUBLIC_KEY_MD5_HASH_PROP_ID = 24;
        private const int CERT_SUBJECT_PUBLIC_KEY_MD5_HASH_PROP_ID = 25;
        private const int CERT_ENROLLMENT_PROP_ID = 26;
        private const int CERT_DATE_STAMP_PROP_ID = 27;
        private const int CERT_ISSUER_SERIAL_NUMBER_MD5_HASH_PROP_ID = 28;
        private const int CERT_SUBJECT_NAME_MD5_HASH_PROP_ID = 29;
        private const int CERT_EXTENDED_ERROR_INFO_PROP_ID = 30;

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        };


        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_NAME_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_OBJID_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        private struct CRYPT_DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_INTEGER_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_BIT_BLOB
        {
            public int cbData;
            public IntPtr pbData;
            public int cUnusedBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_EXTENSION
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pszObjId;
            public int fCritical;
            public CRYPT_OBJID_BLOB Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_EXTENSIONS
        {
            public int cExtension;
            public IntPtr rgExtension;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_AUTHORITY_KEY_ID_INFO
        {
            public CRYPT_DATA_BLOB KeyId;
            public CERT_NAME_BLOB CertIssuer;
            public CRYPT_INTEGER_BLOB CertSerialNumber;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_AUTHORITY_KEY_ID2_INFO
        {
            public CRYPT_DATA_BLOB KeyId;
            public CERT_ALT_NAME_INFO AuthorityCertIssuer;
            public CRYPT_INTEGER_BLOB AuthorityCertSerialNumber;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_BASIC_CONSTRAINTS2_INFO
        {
            public int fCA;
            public int fPathLenConstraint;
            public int dwPathLenConstraint;
        }

        [StructLayout(LayoutKind.Sequential)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        private struct CERT_ENHKEY_USAGE
        {
            public int cUsageIdentifier;
            public IntPtr rgpszUsageIdentifier;
        }

        [StructLayout(LayoutKind.Explicit)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        private struct CERT_ALT_NAME_ENTRY_UNION
        {
            /*
            [FieldOffset(0)]
            public IntPtr pOtherName;         // 1
            [FieldOffset(0)]
            public IntPtr pwszRfc822Name;     // 2  (encoded IA5)
            */

            [FieldOffset(0)]
            public IntPtr pwszDNSName;        // 3  (encoded IA5)

            /*
            [FieldOffset(0)]
            public CERT_NAME_BLOB DirectoryName;      // 5
            */

            [FieldOffset(0)]
            public IntPtr pwszURL;            // 7  (encoded IA5)
            [FieldOffset(0)]
            public CRYPT_DATA_BLOB IPAddress;          // 8  (Octet String)

            /*
            [FieldOffset(0)]
            public IntPtr pszRegisteredID;    // 9  (Object Identifer)
            */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_ALT_NAME_ENTRY
        {
            public int dwAltNameChoice;
            public CERT_ALT_NAME_ENTRY_UNION Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_ALT_NAME_INFO
        {
            public int cAltEntry;
            public IntPtr rgAltEntry;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_ALGORITHM_IDENTIFIER
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pszObjId;
            public CRYPT_OBJID_BLOB Parameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_PUBLIC_KEY_INFO
        {
            public CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            public CRYPT_BIT_BLOB PublicKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_KEY_PROV_INFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszProvName;
            public int dwProvType;
            public int dwFlags;
            public int cProvParam;
            public IntPtr rgProvParam;
            public int dwKeySpec;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_SIGNED_CONTENT_INFO
        {
            public CRYPT_DATA_BLOB ToBeSigned;
            public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public CRYPT_BIT_BLOB Signature;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CRL_INFO
        {
            public int dwVersion;
            public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public CERT_NAME_BLOB Issuer;
            public System.Runtime.InteropServices.ComTypes.FILETIME ThisUpdate;
            public System.Runtime.InteropServices.ComTypes.FILETIME NextUpdate;
            public int cCRLEntry;
            public IntPtr rgCRLEntry;
            public int cExtension;
            public IntPtr rgExtension;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CRL_ENTRY
        {
            public CRYPT_INTEGER_BLOB SerialNumber;
            public System.Runtime.InteropServices.ComTypes.FILETIME RevocationDate;
            public int cExtension;
            public IntPtr rgExtension;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_INFO
        {
            public int dwVersion;
            public CRYPT_INTEGER_BLOB SerialNumber;
            public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public CERT_NAME_BLOB Issuer;
            public System.Runtime.InteropServices.ComTypes.FILETIME NotBefore;
            public System.Runtime.InteropServices.ComTypes.FILETIME NotAfter;
            public CERT_NAME_BLOB Subject;
            public CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
            public CRYPT_BIT_BLOB IssuerUniqueId;
            public CRYPT_BIT_BLOB SubjectUniqueId;
            public int cExtension;
            public IntPtr rgExtension;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_CONTEXT
        {
            public int dwCertEncodingType;
            public IntPtr pbCertEncoded;
            public int cbCertEncoded;
            public IntPtr pCertInfo;
            public IntPtr hCertStore;
        }

        private const int CERT_ALT_NAME_OTHER_NAME = 1;
        private const int CERT_ALT_NAME_RFC822_NAME = 2;
        private const int CERT_ALT_NAME_DNS_NAME = 3;
        private const int CERT_ALT_NAME_X400_ADDRESS = 4;
        private const int CERT_ALT_NAME_DIRECTORY_NAME = 5;
        private const int CERT_ALT_NAME_EDI_PARTY_NAME = 6;
        private const int CERT_ALT_NAME_URL = 7;
        private const int CERT_ALT_NAME_IP_ADDRESS = 8;
        private const int CERT_ALT_NAME_REGISTERED_ID = 9;

        /// <summary>
        /// Declares the native methods used by the class.
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("Kernel32.dll")]
            public static extern void GetSystemTime(ref SYSTEMTIME lpSystemTime);

            [DllImport("Kernel32.dll")]
            public static extern int FileTimeToSystemTime(
                ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime,
                ref SYSTEMTIME lpSystemTime);

            [DllImport("Kernel32.dll")]
            public static extern int GetLastError();

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern int PFXExportCertStoreEx(
                IntPtr hStore,
                ref CRYPT_DATA_BLOB pPFX,
                [MarshalAs(UnmanagedType.LPWStr)]
                string szPassword,
                IntPtr pvReserved,
                int dwFlags);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern IntPtr PFXImportCertStore(
                ref CRYPT_DATA_BLOB pPFX,
                [MarshalAs(UnmanagedType.LPWStr)]
                string szPassword,
                int dwFlags);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern IntPtr CertCreateSelfSignCertificate(
                IntPtr hProv,
                IntPtr pSubjectIssuerBlob,
                int dwFlags,
                IntPtr pKeyProvInfo,
                IntPtr pSignatureAlgorithm,
                IntPtr pStartTime,
                IntPtr pEndTime,
                IntPtr pExtensions);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern int CertGetCertificateContextProperty(
                IntPtr pCertContext,
                int dwPropId,
                IntPtr pvData,
                ref int pcbData);

            [DllImport("Crypt32.dll")]
            public static extern int CertSetCertificateContextProperty(
                IntPtr pCertContext,
                int dwPropId,
                int dwFlags,
                IntPtr pvData);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern int CertAddCertificateContextToStore(
                IntPtr hCertStore,
                IntPtr pCertContext,
                int dwAddDisposition,
                ref IntPtr ppStoreContext);

            [DllImport("Crypt32.dll")]
            public static extern int CertAddCertificateLinkToStore(
                IntPtr hCertStore,
                IntPtr pCertContext,
                int dwAddDisposition,
                ref IntPtr ppStoreContext);

            [DllImport("Crypt32.dll")]
            public static extern int CertFreeCertificateContext(IntPtr pCertContext);

            [DllImport("Crypt32.dll")]
            public static extern IntPtr CertFindCertificateInStore(
                IntPtr hCertStore,
                int dwCertEncodingType,
                int dwFindFlags,
                int dwFindType,
                IntPtr pvFindPara,
                IntPtr pPrevCertContext);

            [DllImport("Crypt32.dll")]
            public static extern IntPtr CertEnumCertificatesInStore(
                IntPtr hCertStore,
                IntPtr pPrevCertContext);

            [DllImport("Crypt32.dll")]
            public static extern int CertDeleteCertificateFromStore(IntPtr pCertContext);

            [DllImport("Advapi32.dll", SetLastError = true)]
            public static extern int CryptGenKey(
                IntPtr hProv,
                int Algid,
                int dwFlags,
                ref IntPtr phKey);

            [DllImport("Advapi32.dll")]
            public static extern int CryptDestroyKey(IntPtr hKey);

            [DllImport("Crypt32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            public static extern int CryptExportPublicKeyInfoEx(
                IntPtr hCryptProv,
                int dwKeySpec,
                int dwCertEncodingType,
                [MarshalAs(UnmanagedType.LPStr)]
                string pszPublicKeyObjId,
                int dwFlags,
                IntPtr pvAuxInfo,
                IntPtr pInfo,
                ref int pcbInfo);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern int CryptHashPublicKeyInfo(
                IntPtr hCryptProv,
                int Algid,
                int dwFlags,
                int dwCertEncodingType,
                IntPtr pInfo,
                IntPtr pbComputedHash,
                ref int pcbComputedHash);

            [DllImport("Crypt32.dll")]
            public static extern int CertStrToNameW(
                int dwCertEncodingType,
                [MarshalAs(UnmanagedType.LPWStr)]
                string pszX500,
                int dwStrType,
                IntPtr pvReserved,
                IntPtr pbEncoded,
                ref int pcbEncoded,
                IntPtr ppszError);

            [DllImport("Crypt32.dll")]
            public static extern int CertNameToStrW(
                int dwCertEncodingType,
                IntPtr pName,
                int dwStrType,
                IntPtr psz,
                int csz);

            [DllImport("Advapi32.dll", SetLastError = true)]
            public static extern int CryptAcquireContextW(
                ref IntPtr phProv,
                [MarshalAs(UnmanagedType.LPWStr)]
                string szContainer,
                [MarshalAs(UnmanagedType.LPWStr)]
                string szProvider,
                int dwProvType,
                int dwFlags);

            [DllImport("Advapi32.dll")]
            public static extern int CryptReleaseContext(
                IntPtr hProv,
                int dwFlags);

            [DllImport("Crypt32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
            public static extern int CryptEncodeObjectEx(
                int dwCertEncodingType,
                [MarshalAs(UnmanagedType.LPStr)]
                string lpszStructType,
                IntPtr pvStructInfo,
                int dwFlags,
                IntPtr pEncodePara,
                IntPtr pvEncoded,
                ref int pcbEncoded);

            [DllImport("Crypt32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            public static extern int CryptDecodeObjectEx(
                int dwCertEncodingType,
                [MarshalAs(UnmanagedType.LPStr)]
                string lpszStructType,
                IntPtr pbEncoded,
                int cbEncoded,
                int dwFlags,
                IntPtr pDecodePara,
                IntPtr pvStructInfo,
                ref int pcbStructInfo);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern IntPtr CertOpenStore(
                IntPtr lpszStoreProvider,
                uint dwEncodingType,
                IntPtr hCryptProv,
                int dwFlags,
                IntPtr pvPara);

            [DllImport("Crypt32.dll")]
            public static extern int CertCloseStore(
                IntPtr hCertStore,
                uint dwFlags);

            [DllImport("Kernel32.dll")]
            public static extern IntPtr LocalFree(IntPtr hMem);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern int CryptGetProvParam(
                IntPtr hProv,
                int dwParam,
                [MarshalAs(UnmanagedType.LPStr)]
                StringBuilder pbData,
                ref int dwDataLen,
                int dwFlags);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool CryptGetUserKey(
                IntPtr hProv,
                uint dwKeySpec,
                ref IntPtr hKey);

            [DllImport("Crypt32.dll", SetLastError = true)]
            public static extern IntPtr CertCreateCRLContext(
                int dwCertEncodingType,
                IntPtr pbCrlEncoded,
                ref int cbCrlEncoded);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPT_ENCODE_PARA
        {
            public int cbSize;
            public IntPtr pfnAlloc;
            public IntPtr pfnFree;
        };

        private const int CERT_STORE_PROV_MEMORY = 2;
        private const int CERT_STORE_PROV_SYSTEM_W = 10;
        private const int CERT_STORE_PROV_SYSTEM = CERT_STORE_PROV_SYSTEM_W;

        private const int CERT_CLOSE_STORE_FORCE_FLAG = 0x00000001;
        private const int CERT_CLOSE_STORE_CHECK_FLAG = 0x00000002;

        #endregion

        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean CertSetCertificateContextProperty(
        [In] IntPtr pCertContext,
        [In] UInt32 dwPropId,
        [In] UInt32 dwFlags,
        [In] IntPtr pvData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CRYPTOAPI_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        public static bool SetClientAuthEKU(X509Certificate2 cert)
        {
            CERT_EXTENSION extension = new CERT_EXTENSION();
            CreateExtendedKeyUsageExtension(ref extension);
            IntPtr pPos = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CERT_EXTENSION)));

            Marshal.StructureToPtr(extension, pPos, false);

            var result = CertSetCertificateContextProperty(cert.Handle, CERT_ENHKEY_USAGE_PROP_ID, 0, extension.Value.pbData);

            Marshal.FreeHGlobal(extension.Value.pbData);
            Marshal.FreeHGlobal(pPos);

            return result;
        }


        //public bool SetEnhancedKeyUsageExtention(List<string> OIDs)
        //{
        //    CERT_EXTENSIONS extensions = new CERT_EXTENSIONS();
        //    CERT_EXTENSION extension = new CERT_EXTENSION();
        //    SYSTEMTIME stValidTo = new SYSTEMTIME();

        //    // allocate memory for all possible extensions.
        //    extensions.cExtension = 0;
        //    extensions.rgExtension = Marshal.AllocHGlobal(6 * Marshal.SizeOf(typeof(CERT_EXTENSION)));

        //    IntPtr pPos = extensions.rgExtension;
        //    CreateExtendedKeyUsageExtension(ref extension);
        //    Marshal.StructureToPtr(extension, pPos, false);
        //    pPos = new IntPtr(pPos.ToInt64() + Marshal.SizeOf(typeof(CERT_EXTENSION)));

        //    // set the expiration date.
        //    DateTime validTo = DateTime.UtcNow.AddMonths(12);
        //    System.Runtime.InteropServices.ComTypes.FILETIME ftValidTo = new System.Runtime.InteropServices.ComTypes.FILETIME();
        //    ulong ticks = (ulong)(validTo.Ticks - new DateTime(1601, 1, 1).Ticks);
        //    ftValidTo.dwHighDateTime = (int)((0xFFFFFFFF00000000 & (ulong)ticks) >> 32);
        //    ftValidTo.dwLowDateTime = (int)((ulong)ticks & 0x00000000FFFFFFFF);

        //    NativeMethods.FileTimeToSystemTime(ref ftValidTo, ref stValidTo);

        //}

        static void CreateExtendedKeyUsageExtension(ref CERT_EXTENSION pExtension)
        {
            // build list of allowed key uses
            IntPtr[] allowedUses = new IntPtr[1];

            allowedUses[0] = Marshal.StringToHGlobalAnsi(szOID_PKIX_KP_CLIENT_AUTH);

            CERT_ENHKEY_USAGE usage;

            usage.cUsageIdentifier = 1;
            usage.rgpszUsageIdentifier = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.Copy(allowedUses, 0, usage.rgpszUsageIdentifier, allowedUses.Length);

            // initialize extension.
            pExtension.pszObjId = szOID_ENHANCED_KEY_USAGE;
            pExtension.fCritical = 1;

            GCHandle hUsage = GCHandle.Alloc(usage, GCHandleType.Pinned);
            IntPtr pData = IntPtr.Zero;
            int dwDataSize = 0;

            try
            {
                // calculate amount of memory required.
                int bResult = NativeMethods.CryptEncodeObjectEx(
                    X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                    szOID_ENHANCED_KEY_USAGE, // X509_ENHANCED_KEY_USAGE,
                    hUsage.AddrOfPinnedObject(),
                    0,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    ref dwDataSize);

                if (bResult == 0)
                {
                    throw new InvalidOperationException("Could not get size for extended key usage extension.");
                }

                // allocate memory.
                pData = Marshal.AllocHGlobal(dwDataSize);

                // encode blob.
                bResult = NativeMethods.CryptEncodeObjectEx(
                    X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                    szOID_ENHANCED_KEY_USAGE, // X509_ENHANCED_KEY_USAGE,
                    hUsage.AddrOfPinnedObject(),
                    0,
                    IntPtr.Zero,
                    pData,
                    ref dwDataSize);

                if (bResult == 0)
                {
                    throw new InvalidOperationException("Could not create extended key usage extension.");
                }

                pExtension.Value.cbData = dwDataSize;
                pExtension.Value.pbData = pData;
                pData = IntPtr.Zero;
            }
            finally
            {
                if (pData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pData);
                }

                Marshal.FreeHGlobal(allowedUses[0]);
                Marshal.FreeHGlobal(usage.rgpszUsageIdentifier);

                if (hUsage.IsAllocated)
                {
                    hUsage.Free();
                }
            }
        }
    }
}
