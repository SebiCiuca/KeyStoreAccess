using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class KeyStoreFactory : IKeyStoreFactory
    {
        private readonly IHttpClient m_HttpClient;
        private readonly ISessionProvider m_SessionProvider;

        public KeyStoreFactory(IHttpClient httpClient, ISessionProvider sessionProvider)
        {
            m_HttpClient = httpClient;
            m_SessionProvider = sessionProvider;
        }

        private KeyStoreManager GetWindowsManager(ISession session)
        {
            var resolver = new WindowsKeyStoreResolver(session, m_HttpClient);

            return new KeyStoreManager(resolver, m_HttpClient, m_SessionProvider);
        }

        public IKeyStoreManager GetKeyStoreManager(string os)
        {
            if (string.IsNullOrEmpty(os))
            {
                throw new ArgumentNullException(nameof(os));
            }

            var session = m_SessionProvider.CurrentSession;

            switch (os)
            {
                case "windows":
                    return GetWindowsManager(session);
                //case "macos":
                //    return new MacOSKeyStoreResolver();
                default:
                    return GetWindowsManager(session);
            }
        }
    }
}
