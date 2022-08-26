using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class KeyStoreFactory : IKeyStoreFactory
    {
        private readonly IHttpClient m_HttpClient;

        public KeyStoreFactory(IHttpClient httpClient)
        {
            m_HttpClient = httpClient;
        }

        public IKeyStoreResolver GetKeyStoreResolver(string os, ISession session)
        {
            if (string.IsNullOrEmpty(os))
            {
                throw new ArgumentNullException(nameof(os));
            }

            switch (os)
            {
                case "windows":
                    return new WindowsKeyStoreResolver(session, m_HttpClient);
                //case "macos":
                //    return new MacOSKeyStoreResolver();
                default:
                    return new WindowsKeyStoreResolver(session, m_HttpClient);
            }
        }
    }
}
