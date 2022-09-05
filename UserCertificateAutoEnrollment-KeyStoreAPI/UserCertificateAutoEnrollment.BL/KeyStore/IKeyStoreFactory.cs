using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public interface IKeyStoreFactory
    {
        /// <summary>
        /// Returns an <see cref="IKeyStoreManager"/> based on input Operation System
        /// Supports xOS/Windows/Linux
        /// </summary>
        /// <param name="os">Operation System</param>
        /// <returns><see cref="IKeyStoreManager"></see></returns>
        IKeyStoreManager GetKeyStoreManager(string os);
    }
}
