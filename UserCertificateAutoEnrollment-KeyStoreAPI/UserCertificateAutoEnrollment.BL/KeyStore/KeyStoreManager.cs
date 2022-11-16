using NLog;
using System.Text.Json;
using System.Text.Json.Serialization;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public class KeyStoreManager : IKeyStoreManager
    {
        private readonly IKeyStoreResolver m_KeyStore;
        private readonly NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();

        public KeyStoreManager(IKeyStoreResolver keyStore)
        {
            m_KeyStore = keyStore;
        }

        public IKeyStoreResolver KeyStoreResolver => m_KeyStore;

        public async Task<CertManagerDTO> GetCertificatesAsync()
        {
            m_Logger.Trace($"Getting list of certiifcates from store");

            var certificates = await m_KeyStore.ListCertificatesAsync();

            var response = new CertManagerDTO
            {
                LocalCertificates = certificates.ToList()
            };

            return response;
        }

        public async Task<string> GetLoggedInUser()
        {
            m_Logger.Trace("Getting from system logged in user");

            try
            {
                return await m_KeyStore.GetLoggedInUser();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, "Error ar retriving logged user");
            }

            return string.Empty;
        }

        public async Task<string> GetEmail()
        {
            m_Logger.Trace("Getting from system logged in user email");

            try
            {
                return await m_KeyStore.GetEmail();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, "Error ar retriving email");
            }

            return string.Empty;
        }

        public async Task SyncCertificatesAsync(string importCerts, string sessionKey)
        {
            m_Logger.Trace("Syncing certificates using PFX file");

            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                m_Logger.Warn($"Could not sync certificates {nameof(sessionKey)} missing");
            }

            var importCertificateDto = JsonSerializer.Deserialize<ImportCertManagerDTO>(importCerts);

            bool syncSuccessfull = await m_KeyStore.ImportCertificatesAsync(importCertificateDto.Pkcs12, sessionKey);

            foreach (var cert in importCertificateDto.ImportCertificates)
            {
                if (cert.IsAuthCertificate)
                {
                    await m_KeyStore.SetAuthKeyUsageExtension(cert.Thumbprint);
                }
            }

            m_Logger.Info($"Syncing certificates result: {syncSuccessfull}.In case sync is not successfully, check previous logs");
        }
    }
}
