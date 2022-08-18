using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;

namespace UserCertificateAutoEnrollment_KeyStoreAPI
{
    public class App
    {
        private readonly IKeyStoreFactory m_KeyStoreFactory;
        private readonly ISessionProvider m_SessionProvider;

        public App(IKeyStoreFactory keyStoreFactory, ISessionProvider sessionProvider)
        {
            m_KeyStoreFactory = keyStoreFactory;
            m_SessionProvider = sessionProvider;
        }

        public async Task RunAsync()
        {
            ISession session = await m_SessionProvider.CreateSession("value");

            
            //var keysStoreResolver = m_KeyStoreFactory.GetKeyStoreResolver("windows");
            //var keys = keysStoreResolver.ListKeys().ToList();
            //keysStoreResolver.AddCertificate("keyName","certificate-sebi-name");
            //var keysAfterNewKey = keysStoreResolver.ListKeys().ToList();
            ////keysStoreResolver.DeleteKey();
            //var keysAfterDeleteKey = keysStoreResolver.ListKeys().ToList();
            ////keysStoreResolver.SetKeyUsage();

            ////foreach (var key in keys)
            ////{
            ////    Console.WriteLine(key);
            ////}
        }
    }
}
