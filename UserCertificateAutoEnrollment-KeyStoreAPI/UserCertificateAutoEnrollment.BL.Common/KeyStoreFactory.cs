using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class KeyStoreFactory : IKeyStoreFactory
    {
        private readonly ISession m_Session;

        public KeyStoreFactory(ISession session)
        {
            m_Session = session;
        }
        public IKeyStoreResolver GetKeyStoreResolver(string os)
        {
            if (string.IsNullOrEmpty(os))
            {
                throw new ArgumentNullException(nameof(os));
            }


            switch (os)
            {
                case "windows":
                    return new WindowsKeyStoreResolver(m_Session);
                //case "macos":
                //    return new MacOSKeyStoreResolver();
                default:
                    return new WindowsKeyStoreResolver(m_Session);
            }
        }
    }
}
