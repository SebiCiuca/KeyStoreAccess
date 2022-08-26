using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public interface IKeyStoreFactory
    {
        /// <summary>
        /// Returns an <see cref="IKeyStoreResolver"/> based on input Operation System
        /// Supports xOS/Windows/Linux
        /// </summary>
        /// <param name="os">Operation System</param>
        /// <returns><see cref="IKeyStoreResolver"></see></returns>
        IKeyStoreResolver GetKeyStoreResolver(string os, ISession session);
    }
}
