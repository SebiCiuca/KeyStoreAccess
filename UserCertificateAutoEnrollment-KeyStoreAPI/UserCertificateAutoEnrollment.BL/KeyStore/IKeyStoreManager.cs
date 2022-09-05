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
        /// Retrieves from Allianz certificates of a specific SST Type and syncs when 
        /// using the <see cref="KeyStoreResolver"/> with the Certificate Store
        /// </summary>
        /// <param name="sSTType">Serialized Certificate Store Type</param>
        /// <returns></returns>
        Task SyncCertificatesAsync(string sSTType);
        /// <summary>
        /// Retrieves list of certificates based on an SST Type
        /// </summary>
        /// <param name="sSTType">Serialized Certificate Store Type</param>
        /// <returns>List of certficates found in Store</returns>
        Task<IEnumerable<CertificateModel>> GetCertificatesAsync(string sSTType);

    }
}
