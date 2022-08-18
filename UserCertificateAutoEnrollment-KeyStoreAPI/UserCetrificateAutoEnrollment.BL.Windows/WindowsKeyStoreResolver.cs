using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UserCertificateAutoEnrollment.BL;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;

namespace UserCetrificateAutoEnrollment.BL.Windows
{
    public class WindowsKeyStoreResolver : IKeyStoreResolver
    {
        private readonly ISession m_Session;

        public WindowsKeyStoreResolver(ISession session)
        {
            m_Session = session;
        }

        public IEnumerable<CertificateModel> ListKeys()
        {
            //open local store 
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            //for listing certificates we only need readonly access
            store.Open(OpenFlags.ReadOnly);

            //load all certificates
            var certificates = store.Certificates.OfType<X509Certificate2>();
            List<CertificateModel> validCertificates = new();

            foreach (var cert in certificates)
            {

                if (!cert.HasPrivateKey)
                {
                    continue;
                }

                var privateKey = cert.GetRSAPrivateKey();

                if (privateKey == null)
                {
                    throw new Exception("Private key should be present, but could not be retrieved");
                }

                RSACng rsa = (RSACng)privateKey;

                //we search only for RSA Keys
                if (rsa == null)
                {
                    throw new NotSupportedException("Private key cannot be used with RSA algorithm");
                }

                validCertificates.Add(new CertificateModel
                {
                    Issuer = cert.Issuer,
                    NotAfter = cert.NotAfter,
                    NotBefore = cert.NotBefore,
                    SerialNumber = cert.SerialNumber,
                    SubjectName = cert.Subject,
                    Provider = rsa.Key?.Provider?.Provider,
                    UniqueIdentifier = rsa.Key.UniqueName
                });
            }

            return validCertificates;
        }

        public void DeleteCertficate(string certificateName)
        {
            if (!certificateName.StartsWith(Constants.CertificateNameStart))
            {
                certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
            }

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);

            var certifcateToRemove = store.Certificates
                .OfType<X509Certificate2>()
                .FirstOrDefault(c => c.FriendlyName == certificateName);

            if (certifcateToRemove != null)
            {

                store.Remove(certifcateToRemove);

            }
            store.Close();
        }

        public void AddCertificate(string keyName, string certificateName)
        {
            CngKey cngKey;

            CngKeyCreationParameters cng = new CngKeyCreationParameters
            {
                KeyUsage = CngKeyUsages.AllUsages
            };


            if (!CngKey.Exists(keyName))
            {
                cngKey = CngKey.Create(CngAlgorithm.Rsa, keyName, cng);
            }
            else
            {
                cngKey = CngKey.Open(keyName);
            }

            RSACng rsaKey = new RSACng(cngKey)
            {
                KeySize = 2048
            };

            List<OidsModel> keyUsageExtension = new List<OidsModel>
                {
                    new OidsModel
                    {
                        FriendlyName = "Indicates that a certificate can be used as an SSL server certificate",
                        //iso(1) iso-identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) pkix(7) kp(3) serverAuth(1)
                        Oid = "1.3.6.1.5.5.7.3.1"
                    },
                    new OidsModel
                    {
                        FriendlyName = "Indicates that a certificate can be used as a Secure Sockets Layer (SSL) client certificate",
                        //iso(1) identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) pkix(7) kp(3) clientAuth(2)
                        Oid = "1.3.6.1.5.5.7.3.2"
                    }
                };

            if (!certificateName.StartsWith(Constants.CertificateNameStart))
            {
                certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
            }

            using X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var certificate = BuildSelfSignedServerCertificate(rsaKey, certificateName, m_Session.SessionKey,
                keyUsageExtension, DateTime.Now.AddDays(-1), DateTime.Now.AddYears(1));

            store.Add(certificate);
            var thumbprint = certificate.Thumbprint;

            store.Close();
        }

        public void SetKeyUsage(string certificateName, List<OidsModel> extenedKeyUsage)
        {
            if (!certificateName.StartsWith(Constants.CertificateNameStart))
            {
                certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
            }

            using X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var certificate = store.Certificates.OfType<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == certificateName);

            var currentExtentions = certificate.Extensions.OfType<X509EnhancedKeyUsageExtension>();

            extenedKeyUsage = extenedKeyUsage.Where(eku => !currentExtentions.Any(ce => ce.Oid.Equals(eku))).ToList();

            foreach (var key in extenedKeyUsage)
            {
                certificate.Extensions.Add(new X509EnhancedKeyUsageExtension
                {
                    Oid = new Oid
                    {
                        Value = key.Oid,
                        FriendlyName = key.FriendlyName
                    },
                    Critical = false
                });
            }

            store.Remove(certificate);
            store.Add(certificate);

            store.Close();
        }

        public bool ValidateCertificate(byte[] codeSignCertBytes, byte[] condeSignCertIssuerBytes, byte[] rcaBytes)
        {
            X509Certificate2 codeSignCert = new X509Certificate2(codeSignCertBytes);

            if (!codeSignCert.Subject.Equals(Constants.ALLIANZ_CERTIFICATE_SUBJECT))
            {
                //TODO Add log here that certificate is invalid ( should not be  added)
                //logger.Error("Code Signing Certificate Subject (" + codeSignCert.Subject + ") not equals expected value : " + GoodSubject);
                return false;
            }

            X509Certificate2 codeSignCertIssuer = new X509Certificate2(condeSignCertIssuerBytes);
            X509Certificate2 RCA = new X509Certificate2(rcaBytes);

            X509Chain chain = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

            chain.ChainPolicy.ExtraStore.Add(codeSignCertIssuer);
            chain.ChainPolicy.ExtraStore.Add(RCA);


            if (!chain.Build(codeSignCert))
            {
                //TODO Add log here
                //logger.Error("Chain building failed.");
                return false;
            }

            var valid = chain.ChainElements
                       .OfType<X509ChainElement>()
                       .Any(x => x.Certificate.Thumbprint == RCA.Thumbprint);

            if (!valid)
            {
                //TODO Log error here
                //logger.Error(RCA.Issuer + " : not found in the Chain.");
                return false;
            }

            return true;
        }

        private X509Certificate2 BuildSelfSignedServerCertificate(RSA key, string certificateName, string password,
            List<OidsModel> keyUsageList, DateTime notBefore, DateTime notAfter)
        {
            if (!certificateName.StartsWith(Constants.CertificateNameStart))
            {
                certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
            }

            X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={certificateName}");

            var request = new CertificateRequest(distinguishedName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

            var oidCollection = new OidCollection();
            foreach (var keyUsage in keyUsageList)
            {
                //TODO : validateKeyUsage OID
                oidCollection.Add(new Oid
                {
                    Value = keyUsage.Oid,
                    FriendlyName = keyUsage.FriendlyName
                });
            }

            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(oidCollection, false));

            var certificate = request.CreateSelfSigned(new DateTimeOffset(notBefore), new DateTimeOffset(notAfter));

            certificate.FriendlyName = certificateName;

            return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet);
        }

        public void AddCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation)
        {
            using X509Store CAStore = new X509Store(storeName, storeLocation);
            CAStore.Open(OpenFlags.ReadWrite);

            if (!FindCertificate(storeName, certificate, storeLocation, CAStore))
            {
                return;
            }

            CAStore.Add(certificate);
            CAStore.Close();
        }

        public void RemoveCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation)
        {
            using X509Store CAStore = new X509Store(storeName, storeLocation);
            CAStore.Open(OpenFlags.ReadWrite);

            if (!FindCertificate(storeName, certificate, storeLocation, CAStore))
            {
                return;
            }

            CAStore.Remove(certificate);
            CAStore.Close();
        }

        private bool FindCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation, X509Store store)
        {
            X509Certificate2Collection foundX509Certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);

            if (foundX509Certificates != null || foundX509Certificates.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
