using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCertificateAutoEnrollment.BL.Session;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public class KeyStoreManager : IKeyStoreManager
    {
        private readonly IKeyStoreResolver m_KeyStore;
        private readonly IHttpClient m_HttpClient;
        private readonly NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();
        private readonly ISession m_CurrentSession;

        public KeyStoreManager(IKeyStoreResolver keyStore, IHttpClient httpClient, ISessionProvider sessionProvider)
        {
            m_KeyStore = keyStore;
            m_HttpClient = httpClient;
            m_CurrentSession = sessionProvider.CurrentSession;
        }

        public IKeyStoreResolver KeyStoreResolver => m_KeyStore;

        public async Task<IEnumerable<CertificateModel>> GetCertificatesAsync(string sSTType)
        {
            SSTTypesEnum inputsSTType = (SSTTypesEnum)Enum.Parse(typeof(SSTTypesEnum), sSTType);

            m_Logger.Trace($"Getting list of certiifcates from store for SST Type {sSTType}");

            var certificates = await m_KeyStore.ListCertificatesAsync(sSTType);

            return certificates;
        }

        public async Task SyncCertificatesAsync(string sSTType)
        {
            SSTTypesEnum inputsSTType = (SSTTypesEnum)Enum.Parse(typeof(SSTTypesEnum), sSTType);

            var sstTypeUrl = Constants.SSTTypes[inputsSTType];

            //var sstName = Enum.GetName(typeof(SSTTypesEnum), sstType.Key);
            var sstUrl = sstTypeUrl;
            m_Logger.Info($"Processing SST: {sSTType}");

            //add settings
            //if (inputsSTType == SSTTypesEnum.INTERCEPTION && !m_RegistrySettings.AllowInterception)
            if (inputsSTType == SSTTypesEnum.INTERCEPTION && false)
            {
                return;
            }

            m_Logger.Trace("Checking if session has trust certificate");
            if (m_CurrentSession.TrustCert == null)
            {
                m_Logger.Trace("No trust certificate found for current session");
                m_Logger.Info("Intialize trust certificate");
                await m_KeyStore.InitializeTrust();
            }

            var certificateBytes = await m_HttpClient.GetCertificate(sstUrl);
            var certifcatesBytesSignature = await m_HttpClient.GetCertificateSignature(sstUrl);

            var certificateOk = m_KeyStore.VerifySignature(certificateBytes, certifcatesBytesSignature);

            if (certificateOk)
            {
                m_Logger.Info("Signature verification passed, add certificates to the list for processing.");

                await m_KeyStore.ImportCertificate(certificateBytes, sSTType);
            }
        }
    }
}
