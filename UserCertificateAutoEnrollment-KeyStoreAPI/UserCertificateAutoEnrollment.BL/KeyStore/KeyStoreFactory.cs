using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class KeyStoreFactory : IKeyStoreFactory
    {
        public KeyStoreFactory()
        {
        }

        private KeyStoreManager GetWindowsManager()
        {
            var resolver = new WindowsKeyStoreResolver();

            return new KeyStoreManager(resolver);
        }

        public IKeyStoreManager GetKeyStoreManager(string os)
        {
            if (string.IsNullOrEmpty(os))
            {
                throw new ArgumentNullException(nameof(os));
            }

            switch (os)
            {
                case "windows":
                    return GetWindowsManager();
                default:
                    return GetWindowsManager();
            }
        }
    }
}
