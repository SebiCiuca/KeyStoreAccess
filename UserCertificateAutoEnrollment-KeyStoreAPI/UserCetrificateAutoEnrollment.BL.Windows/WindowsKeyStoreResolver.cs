using NLog;
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

        private List<string> m_Issuers = new List<string> {
            "Allianz UserCA",
            "Allianz Smartcard CA",
            "Allianz Dresdner CA",
            "Aussendienst Smartcard CA",
            "Allianz UserCA V",
            "Allianz Smartcard CA V",
            "Allianz Infrastructure CA V"
        };

        private IEnumerable<CertificateDTO> GetStoreCertificates(StoreName name, StoreLocation location)
        {
            //open local store 
            using var store = new X509Store(name, location);

            //for listing certificates we only need readonly access
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            List<X509Certificate2> allianzCertificates = new();

            //load all certificates
            foreach (var issuer in m_Issuers)
            {
                allianzCertificates.AddRange(store.Certificates.Find(X509FindType.FindByIssuerName, issuer, false).OfType<X509Certificate2>());
            }

            List<CertificateDTO> validCertificates = new();
            var ekusString = new List<string>();

            foreach (var cert in allianzCertificates)
            {
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
            validCertificates.AddRange(GetStoreCertificates(StoreName.Root, StoreLocation.CurrentUser));

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

    }
}