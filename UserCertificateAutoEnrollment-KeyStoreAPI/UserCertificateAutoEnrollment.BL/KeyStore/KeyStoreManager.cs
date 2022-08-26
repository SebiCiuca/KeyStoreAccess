using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public class KeyStoreManager : IKeyStoreManager
    {
        private readonly IKeyStoreResolver m_KeyStore;
        private readonly IHttpClient m_HttpClient;
        private readonly NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();

        public KeyStoreManager(IKeyStoreResolver keyStore, IHttpClient httpClient)
        {
            m_KeyStore = keyStore;
            m_HttpClient = httpClient;
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
