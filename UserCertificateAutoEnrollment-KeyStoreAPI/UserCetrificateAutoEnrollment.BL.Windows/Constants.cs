using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCetrificateAutoEnrollment.BL.Windows
{
    public static class Constants
    {
        internal const string ALLIANZ_CERTIFICATE_SUBJECT = "CN=AZ Tech PKI Trust Signing, OU=TrustManager, O=Allianz, C=DE";

        public static string TRUST_SIGN_LOCATION = "http://rootca.allianz.com/trust/TrustSign.cer";
        public static string TRUST_SIGN_ISSUER_LOCATION = "http://rootca.allianz.com/trust/TrustSignIssuer.cer";
        public const string RCA64 = "MIIGZDCCBEygAwIBAgIBATANBgkqhkiG9w0BAQsFADA9MQswCQYDVQQGEwJERTEQMA4GA1UECh" +
            "MHQWxsaWFuejEcMBoGA1UEAxMTQWxsaWFueiBSb290IENBIElJSTAeFw0xNTA0MjkwODQxMzBaFw00MDA0MjIwODQxMzBaMD0xCz" +
            "AJBgNVBAYTAkRFMRAwDgYDVQQKEwdBbGxpYW56MRwwGgYDVQQDExNBbGxpYW56IFJvb3QgQ0EgSUlJMIICIjANBgkqhkiG9w0BAQ" +
            "EFAAOCAg8AMIICCgKCAgEA1wIec4qvCCKkuzlOfWZV8ny+rdqp9ySUDwnaaRet1TQcmkBe9Bac6CsvxAtTb5kh8O0Y7paFKeYuDL" +
            "U40hqvJiq6a8nlewYAgMKx7H5ETRNgLTkbLbLdJWDWkEU0YL9GRzp76oTgvEFXENh+m8UBFoLNrp/szH2TrCG1Xi3sECABdPIfe+" +
            "dyhyG5fTBEDuuSI1eK+r5LMsW80kB1gb3YITFJlkgKFyHKvHnV6KHa/H4JRxh1y12RW7UctNezcmAYdtfMgvTCGByXP3t1Himzv8" +
            "QM/7cddqcI5vW32E1/WVKlmR89aBjKv5EWGaIZFY2QItH+rIh5rNI+Hk+sTEXE7RrrISSFjOLlgbVNDrYtfg5nv4HKdIXWG8OX5r" +
            "Xfc7W5ZU1WyWLvF1fB0neRwAMC8bLJ8laQHJdOrrITPgiDm7owiii8QlduRjSJyDvzSDxHH/WCA4Sdfb20tT9v9yyZLJMsLqRyFL" +
            "xRTrnhoGURy3/IiYY0495urNYCWcm9K3kajf+psgPgSMdiR8/o4OCXQ3GHfPYMBMDuPDqHLk16Z6WxXJ26Bt1gwi9qh756SHljyf" +
            "MCbHiLFRdg525ivHPXP3P7DXaAfNegJmyo+fQ+yJSIZ5bb0wlVqKtMNfrFMbfOmMJ9CnU8s6jumOFoYTbdRPKsgywhVPcaNZExNR" +
            "2rGkcCAwEAAaOCAW0wggFpMA4GA1UdDwEB/wQEAwIBBjAdBgNVHQ4EFgQUGlfYY4Gxnxr+izZs0KeAaEcuevkwHwYDVR0jBBgwFo" +
            "AUGlfYY4Gxnxr+izZs0KeAaEcuevkwEgYDVR0TAQH/BAgwBgEB/wIBATARBglghkgBhvhCAQEEBAMCAAcwOgYDVR0fBDMwMTAvoC" +
            "2gK4YpaHR0cDovL3Jvb3RjYS5hbGxpYW56LmNvbS9jcmwvcm9vdGNhMy5jcmwwgbMGA1UdIASBqzCBqDCBpQYJKwYBBAG3dx4DMI" +
            "GXMCsGCCsGAQUFBwIBFh9odHRwOi8vcm9vdGNhLmFsbGlhbnouY29tL2NwczMvMGgGCCsGAQUFBwICMFwwFhYPQWxsaWFueiBHZX" +
            "JtYW55MAMCAQEaQlRoaXMgQ2VydGlmaWNhdGUgaXMgaXNzdWVkIGJ5IEFsbGlhbnogUm9vdCBDQSBJSUksIEFsbGlhbnogR2VybW" +
            "FueTANBgkqhkiG9w0BAQsFAAOCAgEACWT50HEOLlynMkRHl2Xs/cr/bZCQ082wDG5rjw1LMWDaOtYxGg8KQhLG6S52ISEMOHsxxM" +
            "BHHfmryos50Zp/xrg7VX/05qz8K4mop6SGNa/8GbIcRIUaNjuNWhFG+nO9mjZ3sQsJCxW30JgTKgPdZa21S37bBf5u5TWzw7lAND" +
            "9RRr4buwKPMiikJka6q7sIYyagl2xlNrmsz1BJPltPrj9j6nWHt5KD+ITIRRZJo06ib12vGlZfh0Aa9kA1FP3XrbSFUD95un4Kmw" +
            "/re6s/bc7xbyh8/4MjSIl+8TthyevCrA7PSCWiH/8xGbt+kXpm9zIOo9eOyC/s7NnMw7LR98sjEC5rZdbJeeDHMoe/Ps1RsV6vlM" +
            "XM5+ItMCNoQoe+7NkUXsEOf7Gg+jh/PhXNl73Uja2+1S9Ju8U0QAr3RszeCfAtJ+dt6gK1oGPyjsj395jjsQGmKo8GnaqaDXOG9h" +
            "irzLleFO3467++A1G/bLwN9f2qCSGIQJR2UM/0CgSOumE0WNnLK5xADJ1pzR3QN3AQod6miQcc5A2D3a9gFtcE7S5k21JR2BacLv" +
            "Pgab4E+Vc4uaaT156xZ4KsQme2gkpFqygG0d2qSHT4ls5itZa+pzyBxOg5q+yzzme68VUIJX4e0TCGKIM3nWfUvFQN8aVUWoi9Vk" +
            "YEgC1QyvUxgOU=";
    }
}
