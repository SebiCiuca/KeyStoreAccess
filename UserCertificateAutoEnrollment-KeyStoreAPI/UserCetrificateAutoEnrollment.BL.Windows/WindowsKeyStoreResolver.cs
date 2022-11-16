using NLog;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCetrificateAutoEnrollment.BL.Windows
{
    public class WindowsKeyStoreResolver : IKeyStoreResolver
    {
        private readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<CertificateDTO> GetStoreCertificates(StoreName name, StoreLocation location)
        {
            //open local store 
            using var store = new X509Store(name, location);

            //for listing certificates we only need readonly access
            store.Open(OpenFlags.ReadOnly);

            //load all certificates
            var certificates = store.Certificates.OfType<X509Certificate2>();
            List<CertificateDTO> validCertificates = new();
            var ekusString = new List<string>();

            foreach (var cert in certificates)
            {

                //if (!cert.HasPrivateKey)
                //{
                //    continue;
                //}

                RSACng rsa = null;

                try
                {
                    var privateKey = cert.GetRSAPrivateKey();

                    if (privateKey == null)
                    {
                        throw new Exception("Private key should be present, but could not be retrieved");
                    }

                    rsa = (RSACng)privateKey;

                    //we search only for RSA Keys
                    if (rsa == null)
                    {
                        throw new NotSupportedException("Private key cannot be used with RSA algorithm");
                    }

                    var ekus = cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault()?.EnhancedKeyUsages;
                    foreach (var eku in ekus)
                    {
                        ekusString.Add(eku.Value);
                    }

                }
                catch (Exception ex)
                {
                    m_Logger.Error($"Could not retrieve private key for certificate {cert.Subject}.\n" +
                        $"Error message: {ex.Message}");
                }

                validCertificates.Add(new CertificateDTO
                {
                    Issuer = cert.Issuer,
                    NotAfter = cert.NotAfter.ToLongDateString(),
                    SerialNumber = cert.SerialNumber,
                    Subject = cert.Subject,
                    Thumbprint = cert.Thumbprint,
                    EKU = ekusString
                });

                ekusString.Clear();
            }


            store.Close();

            return validCertificates;
        }

        private string GetLoggedInUserEmail()
        {
            return System.DirectoryServices.AccountManagement.UserPrincipal.Current.EmailAddress;
        }

        private string GetFullPassowrd(string passwordUniqueChars)
        {
            return $"{passwordUniqueChars}{GetLoggedInUserEmail()}";
        }
        private X509Certificate2 FindCertificate(string thumbPrint, X509Store store)
        {
            X509Certificate2Collection foundX509Certificates = store.Certificates.Find(X509FindType.FindBySerialNumber, thumbPrint, false);

            if (foundX509Certificates != null || foundX509Certificates.Count > 0)
            {
                return foundX509Certificates.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<CertificateDTO>> ListCertificatesAsync()
        {
            List<CertificateDTO> validCertificates = new();

            validCertificates.AddRange(GetStoreCertificates(StoreName.My, StoreLocation.CurrentUser));
            //validCertificates.AddRange(GetStoreCertificates(StoreName.My, StoreLocation.LocalMachine));
            //validCertificates.AddRange(GetStoreCertificates(StoreName.Root, StoreLocation.CurrentUser));
            //validCertificates.AddRange(GetStoreCertificates(StoreName.Root, StoreLocation.LocalMachine));

            return validCertificates;
        }

        public Task<string> GetLoggedInUser() => Task.FromResult(WindowsIdentity.GetCurrent().Name);
        public Task<string> GetEmail() => Task.FromResult(GetLoggedInUserEmail());

        public Task<bool> ImportCertificatesAsync(byte[] pfxFile, string passwordUniqueChars)
        {
            bool isSuccessfull = false;
            try
            {
                var password = GetFullPassowrd(passwordUniqueChars);

                X509Certificate2 cert = new X509Certificate2(pfxFile, password, X509KeyStorageFlags.PersistKeySet);

                using X509Store store = new X509Store(StoreName.My);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();

                isSuccessfull = true;
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, $"Exception at importing pfx file: {ex.Message}");
            }

            return Task.FromResult(isSuccessfull);
        }

        public async Task<bool> SetAuthKeyUsageExtension(string certThumbprint)
        {
            using X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);

            try
            {
                var certificate = FindCertificate(certThumbprint, store);
                bool certificateUpdateSuccesfully = false;

                if (certificate != null)
                {
                    certificateUpdateSuccesfully = WindowsCertificateFactory.SetClientAuthEKU(certificate);
                }

                if (certificateUpdateSuccesfully)
                {
                    var allianzCertificates = await ListCertificatesAsync();
                    foreach (var cert in allianzCertificates)
                    {
                        var toRemoveClienAuthcertificate = FindCertificate(cert.SerialNumber, store);

                        if (toRemoveClienAuthcertificate != null && toRemoveClienAuthcertificate.HasCertificateClientAuthEKU())
                        {
                            WindowsCertificateFactory.RemoveClientAuthEKU(toRemoveClienAuthcertificate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                store.Close();
            }

            return false;
        }


        //public void DeleteCertficate(string certificateName)
        //{
        //    if (!certificateName.StartsWith(Constants.CertificateNameStart))
        //    {
        //        certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
        //    }

        //    X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        //    store.Open(OpenFlags.ReadWrite);

        //    var certifcateToRemove = store.Certificates
        //        .OfType<X509Certificate2>()
        //        .FirstOrDefault(c => c.FriendlyName == certificateName);

        //    if (certifcateToRemove != null)
        //    {

        //        store.Remove(certifcateToRemove);

        //    }
        //    store.Close();
        //}

        //public void SyncCertificates(string keyName, string certificateName)
        //{
        //    CngKey cngKey;

        //    CngKeyCreationParameters cng = new CngKeyCreationParameters
        //    {
        //        KeyUsage = CngKeyUsages.AllUsages
        //    };


        //    if (!CngKey.Exists(keyName))
        //    {
        //        cngKey = CngKey.Create(CngAlgorithm.Rsa, keyName, cng);
        //    }
        //    else
        //    {
        //        cngKey = CngKey.Open(keyName);
        //    }

        //    RSACng rsaKey = new RSACng(cngKey)
        //    {
        //        KeySize = 2048
        //    };

        //    List<OidsModel> keyUsageExtension = new List<OidsModel>
        //        {
        //            new OidsModel
        //            {
        //                FriendlyName = "Indicates that a certificate can be used as an SSL server certificate",
        //                //iso(1) iso-identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) pkix(7) kp(3) serverAuth(1)
        //                Oid = "1.3.6.1.5.5.7.3.1"
        //            },
        //            new OidsModel
        //            {
        //                FriendlyName = "Indicates that a certificate can be used as a Secure Sockets Layer (SSL) client certificate",
        //                //iso(1) identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) pkix(7) kp(3) clientAuth(2)
        //                Oid = "1.3.6.1.5.5.7.3.2"
        //            }
        //        };

        //    if (!certificateName.StartsWith(Constants.CertificateNameStart))
        //    {
        //        certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
        //    }

        //    using X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadWrite);
        //    var certificate = BuildSelfSignedServerCertificate(rsaKey, certificateName, m_Session.SessionKeyUTF8,
        //        keyUsageExtension, DateTime.Now.AddDays(-1), DateTime.Now.AddYears(1));

        //    store.Add(certificate);
        //    var thumbprint = certificate.Thumbprint;

        //    store.Close();
        //}




        //private bool ValidateCertificate(byte[] codeSignCertBytes, byte[] condeSignCertIssuerBytes)
        //{
        //    X509Certificate2 codeSignCert = new X509Certificate2(codeSignCertBytes);

        //    if (!codeSignCert.Subject.Equals(Constants.ALLIANZ_CERTIFICATE_SUBJECT))
        //    {
        //        m_Logger.Error($"Code Signing Certificate Subject ({codeSignCert.Subject}) not equals expected " +
        //            $"value: {Constants.ALLIANZ_CERTIFICATE_SUBJECT}");

        //        return false;
        //    }

        //    X509Certificate2 codeSignCertIssuer = new X509Certificate2(condeSignCertIssuerBytes);
        //    X509Certificate2 RCA = new X509Certificate2(Convert.FromBase64String(Constants.RCA64));

        //    X509Chain chain = new X509Chain();

        //    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        //    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

        //    chain.ChainPolicy.ExtraStore.Add(codeSignCertIssuer);
        //    chain.ChainPolicy.ExtraStore.Add(RCA);


        //    if (!chain.Build(codeSignCert))
        //    {
        //        m_Logger.Error("Chain building failed.");
        //        return false;
        //    }

        //    var valid = chain.ChainElements
        //               .OfType<X509ChainElement>()
        //               .Any(x => x.Certificate.Thumbprint == RCA.Thumbprint);

        //    if (!valid)
        //    {
        //        m_Logger.Error($"{RCA.Issuer} - not found in the Chain.");

        //        return false;
        //    }

        //    m_Session.InializeTrustCert(codeSignCert.GetRawCertData());

        //    return true;
        //}

        //private bool IsRoot(X509Certificate2 cert2Trust)
        //{
        //    m_Logger.Trace($"Check Certificate({cert2Trust.Subject}|{cert2Trust.Issuer}) Root or Sub.");
        //    X509Extension subjectKeyID = cert2Trust.Extensions["Subject Key Identifier"];
        //    X509Extension authorityKeyID = cert2Trust.Extensions["Authority Key Identifier"];
        //    m_Logger.Trace($"IssuerName ({cert2Trust.IssuerName}); SubjectName ({cert2Trust.SubjectName})");
        //    m_Logger.Trace($"subjectKeyID ({subjectKeyID}); authorityKeyID ({authorityKeyID})");
        //    if (authorityKeyID == null)
        //    {
        //        m_Logger.Trace("AuthorityKeyID missing");
        //        try
        //        {
        //            if (cert2Trust.IssuerName.Name.Equals(cert2Trust.SubjectName.Name))
        //            {
        //                m_Logger.Trace("Issuer and subject same. is Root Certificate.");

        //                return true;
        //            }
        //        }
        //        catch
        //        {
        //            m_Logger.Warn("IssuerName and SubjectName check threw exception, checking with Issuer/Subject directly.");
        //            if (cert2Trust.Issuer.Equals(cert2Trust.Subject))
        //            {
        //                m_Logger.Trace("Issuer and subject same. is Root Certificate.");

        //                return true;
        //            }
        //        }

        //        m_Logger.Trace("Issuer and subject not same. Sub CA.");
        //    }
        //    else if (subjectKeyID == null)
        //    {
        //        m_Logger.Trace("SubjectKeyID missing");
        //        try
        //        {
        //            if (cert2Trust.IssuerName.Name.Equals(cert2Trust.SubjectName.Name))
        //            {
        //                m_Logger.Trace("Issuer and subject same. is Root Certificate.");

        //                return true;
        //            }
        //        }
        //        catch
        //        {
        //            m_Logger.Warn("IssuerName and SubjectName check threw exception, checking with Issuer/Subject directly.");
        //            if (cert2Trust.Issuer.Equals(cert2Trust.Subject))
        //            {
        //                m_Logger.Trace("Issuer and subject same. is Root Certificate.");

        //                return true;
        //            }
        //        }

        //        m_Logger.Trace("Issuer and subject not same. Sub CA.");
        //    }
        //    else if (subjectKeyID.RawData.SequenceEqual(authorityKeyID.RawData))
        //    {
        //        m_Logger.Trace("subjectKeyID and authorityKeyID match.");

        //        return true;
        //    }
        //    return false;
        //}

        //private StoreName GetStoreName(bool isRoot, SSTTypesEnum sSTType)
        //{
        //    switch (sSTType)
        //    {
        //        case SSTTypesEnum.AUTHROOTS:
        //            return StoreName.AuthRoot;
        //        //case SSTTypesEnum.INTERCEPTION:
        //        //    return StoreName.My;
        //        case SSTTypesEnum.UPDROOTS:
        //            return isRoot ? StoreName.AuthRoot : StoreName.CertificateAuthority;
        //        case SSTTypesEnum.ROOTS:
        //            return isRoot ? StoreName.Root : StoreName.CertificateAuthority;
        //        case SSTTypesEnum.DISALLOWEDCERT:
        //            return StoreName.Disallowed;
        //        case SSTTypesEnum.DELROOTS:
        //            return isRoot ? StoreName.Root : StoreName.CertificateAuthority;
        //        case SSTTypesEnum.TRUSTEDPUBLISHER:
        //            return StoreName.TrustedPublisher;
        //        // For removing Publishers
        //        case SSTTypesEnum.UNTRUSTEDPUBLISHER:
        //            return StoreName.TrustedPublisher;
        //        case SSTTypesEnum.DELDISALLOWED:
        //            // Remove from Disallowed
        //            return StoreName.Disallowed;
        //        default:
        //            return StoreName.My;
        //    }
        //}
        //public bool VerifySignature(byte[] certificateBytes, byte[] certifcatesBytesSignature)
        //{
        //    X509Certificate2? codeSignCert = null;
        //    try
        //    {
        //        codeSignCert = new X509Certificate2(m_Session.TrustCert);
        //    }
        //    catch (Exception ex)
        //    {
        //        m_Logger.Error(ex, $"Could not initialize Code Sign Certificate, abording...");

        //        throw;
        //    }

        //    if (codeSignCert == null)
        //    {
        //        m_Logger.Error($"Initializing Code Sign Certificate passed, but for some reason certificate is null...");

        //        return false;
        //    }

        //    RSAParameters rsaParams = codeSignCert.GetRSAPublicKey().ExportParameters(false);
        //    var rsaKey = RSA.Create(rsaParams);
        //    byte[] sha512HashedData;
        //    m_Logger.Trace("Signature data verification started.");
        //    bool dataOK = rsaKey.VerifyData(certificateBytes, certifcatesBytesSignature,
        //        HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);


        //    if (dataOK)
        //    {
        //        m_Logger.Trace("Signature data verification successful.");
        //        using SHA512 sha256Hash = SHA512.Create();
        //        sha512HashedData = sha256Hash.ComputeHash(certificateBytes);
        //        m_Logger.Trace("Signature hash verification started.");
        //        bool hashOK = rsaKey.VerifyHash(sha512HashedData, certifcatesBytesSignature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        //        if (hashOK)
        //        {
        //            m_Logger.Trace("Signature hash verification successful.");

        //            return true;
        //        }

        //        m_Logger.Trace("Signature hash verification failed.");

        //        return false;
        //    }

        //    m_Logger.Trace("Signature data verification failed.");

        //    return false;
        //}



        //public void AddCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine)
        //{
        //    using X509Store CAStore = new X509Store(storeName, storeLocation);
        //    CAStore.Open(OpenFlags.ReadWrite);

        //    if (!FindCertificate(storeName, certificate, storeLocation, CAStore))
        //    {
        //        return;
        //    }

        //    CAStore.Add(certificate);
        //    CAStore.Close();
        //}

        //public void RemoveCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine)
        //{
        //    using X509Store CAStore = new X509Store(storeName, storeLocation);
        //    CAStore.Open(OpenFlags.ReadWrite);

        //    if (!FindCertificate(storeName, certificate, storeLocation, CAStore))
        //    {
        //        return;
        //    }

        //    CAStore.Remove(certificate);
        //    CAStore.Close();
        //}

        //public async Task<bool> InitializeTrust()
        //{
        //    return false;

        //    //m_Logger.Info("Initializing trust certificate");

        //    //if (m_Session.TrustCert != null)
        //    //{
        //    //    return false;
        //    //}

        //    //m_Logger.Info("Retriving trust cert info from server");
        //    //m_Logger.Trace($"Retriving trust cert from {Constants.TRUST_SIGN_LOCATION}");
        //    //m_Logger.Trace($"Retriving trust cert issuer from {Constants.TRUST_SIGN_ISSUER_LOCATION}");

        //    //var trustCertBytes = await m_HttpClient.GetCertificate(Constants.TRUST_SIGN_LOCATION);
        //    //var trustCertIssuer = await m_HttpClient.GetCertificate(Constants.TRUST_SIGN_ISSUER_LOCATION);

        //    //m_Logger.Trace($"Trust cert bytes {trustCertBytes}");
        //    //m_Logger.Trace($"Trust cert issuer bytes {trustCertIssuer}");
        //    //m_Logger.Info($"Successfully retrieved trust certificate, doing validation");

        //    //var result = ValidateCertificate(trustCertBytes, trustCertIssuer);

        //    //if (!result)
        //    //{
        //    //    m_Logger.Info("Trust certificate is not valid, won't be added to session");
        //    //}

        //    //m_Logger.Info("Trust certificate is valid, adding it to session");
        //    //m_Logger.Trace($"Adding valid trust certificat to session with key{m_Session.SessionKey}");

        //    //return result;
        //}

        //public Task<bool> ImportCertificate(byte[] certificateByte, string sSTType)
        //{
        //    SSTTypesEnum inputsSTType = (SSTTypesEnum)Enum.Parse(typeof(SSTTypesEnum), sSTType);
        //    X509Certificate2Collection trustedCertificatesCollection = new X509Certificate2Collection();

        //    trustedCertificatesCollection.Import(certificateByte);


        //    m_Logger.Info($"Got collection from URL: {trustedCertificatesCollection.Count}");

        //    foreach (X509Certificate2 trustedCertificate in trustedCertificatesCollection)
        //    {
        //        bool root = IsRoot(trustedCertificate);
        //        StoreName storeName = GetStoreName(root, inputsSTType);

        //        switch (inputsSTType)
        //        {
        //            case SSTTypesEnum.UPDROOTS:
        //                // Add to Trusted
        //                // Local Machine & CurrentUser
        //                AddCertificate(storeName, trustedCertificate);
        //                // Remove from Untrusted to ensure conflict (LM and CU)
        //                RemoveCertificate(StoreName.Disallowed, trustedCertificate);
        //                break;
        //            case SSTTypesEnum.ROOTS:
        //                AddCertificate(storeName, trustedCertificate);
        //                break;
        //            case SSTTypesEnum.DISALLOWEDCERT:
        //                // Add to Disallowed
        //                AddCertificate(storeName, trustedCertificate);
        //                break;
        //            case SSTTypesEnum.DELROOTS:
        //                RemoveCertificate(storeName, trustedCertificate);
        //                break;
        //            case SSTTypesEnum.TRUSTEDPUBLISHER:
        //                AddCertificate(storeName, trustedCertificate);
        //                break;
        //            // For removing Publishers
        //            case SSTTypesEnum.UNTRUSTEDPUBLISHER:
        //                RemoveCertificate(storeName, trustedCertificate);
        //                break;
        //            case SSTTypesEnum.DELDISALLOWED:
        //                // Remove from Disallowed
        //                RemoveCertificate(storeName, trustedCertificate);
        //                break;
        //        }
        //    }

        //    return Task.FromResult(true);
        //}
        ////private X509Certificate2 BuildSelfSignedServerCertificate(RSA key, string certificateName, string password,
        ////    List<OidsModel> keyUsageList, DateTime notBefore, DateTime notAfter)
        ////{
        ////    if (!certificateName.StartsWith(Constants.CertificateNameStart))
        ////    {
        ////        certificateName = string.Concat(Constants.CertificateNameStart, certificateName);
        ////    }

        ////    X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={certificateName}");

        ////    var request = new CertificateRequest(distinguishedName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        ////    request.CertificateExtensions.Add(
        ////        new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

        ////    var oidCollection = new OidCollection();
        ////    foreach (var keyUsage in keyUsageList)
        ////    {
        ////        //TODO : validateKeyUsage OID
        ////        oidCollection.Add(new Oid
        ////        {
        ////            Value = keyUsage.Oid,
        ////            FriendlyName = keyUsage.FriendlyName
        ////        });
        ////    }

        ////    request.CertificateExtensions.Add(
        ////        new X509EnhancedKeyUsageExtension(oidCollection, false));

        ////    var certificate = request.CreateSelfSigned(new DateTimeOffset(notBefore), new DateTimeOffset(notAfter));

        ////    certificate.FriendlyName = certificateName;

        ////    return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet);
        ////}

    }
}
