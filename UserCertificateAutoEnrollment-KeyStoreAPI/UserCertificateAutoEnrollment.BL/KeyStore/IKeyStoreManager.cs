using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public interface IKeyStoreManager
    {
        /// <summary>
        /// Key Store Resolver, instantiate an object that can directly communicate with
        /// key store. Should be different for each OS
        /// </summary>
        IKeyStoreResolver KeyStoreResolver { get; }
       
        /// <summary>
        /// Gets a list of certificates to import on current user environemnt and also 
        /// gets the certificate that should be set for ClientAuth
        /// </summary>
        /// <param name="importCert"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        Task SyncCertificatesAsync(string importCert, string sessionKey);
        /// <summary>
        /// Retrieve all certificates that are allianz related
        /// </summary>
        /// <param name="sSTType">Serialized Certificate Store Type</param>
        /// <returns>List of certficates found in Store</returns>
        Task<CertManagerDTO> GetCertificatesAsync();

        /// <summary>
        /// Retrieve current logged in user
        /// </summary>
        /// <returns></returns>
        Task<string> GetLoggedInUser();

        /// <summary>
        /// Retrieve current user email
        /// </summary>
        /// <returns></returns>
        Task<string> GetEmail();

    }
}
