using NLog;
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

        public async Task<IEnumerable<CertificateModel>> GetCertificatesAsync()
        {
            m_Logger.Trace($"Getting list of certiifcates from store");

            var certificates = await m_KeyStore.ListCertificatesAsync();

            return certificates;
        }

        public async Task<string> GetLoggedInUser()
        {
            m_Logger.Trace("Getting from system logged in user");

            var loggedInUser = await m_KeyStore.GetLoggedInUser();

            return loggedInUser;
        }

        public async Task SyncCertificatesAsync(byte[] rawData, string sessionKey)
        {
            m_Logger.Trace("Syncing certificates using PFX file");

            bool syncSuccessfull = await m_KeyStore.ImportCertificatesAsync(rawData, sessionKey);

            m_Logger.Info($"Syncing certificates result: {syncSuccessfull}.In case sync is not successfully, check previous logs");
        }
    }
}
