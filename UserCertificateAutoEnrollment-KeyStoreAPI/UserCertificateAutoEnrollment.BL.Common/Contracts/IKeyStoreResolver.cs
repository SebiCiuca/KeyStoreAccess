using System.Security.Cryptography.X509Certificates;

namespace UserCertificateAutoEnrollment.BL.Common.Contracts
{
    public interface IKeyStoreResolver
    {
        /// <summary>
        /// List certificates saved on client machine
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CertificateDTO>> ListCertificatesAsync();

        /// <summary>
        /// Returns logged in user
        /// </summary>
        /// <returns></returns>
        Task<string> GetLoggedInUser();
        /// <summary>
        /// Returns current logged in user email if logged in in AD
        /// </summary>
        /// <returns></returns>
        Task<string> GetEmail();

        /// <summary>
        /// Imports a PFX file recieved as byte[] into Certificate Store under Current User
        /// </summary>
        /// <param name="pfxFile">PFX Raw Data</param>
        /// <param name="passwordUniqueChars">PFX Password</param>
        /// <returns></returns>
        Task<bool> ImportCertificatesAsync(byte[] pfxFile, string passwordUniqueChars);

        /// <summary>
        /// retrives a certificate from store and set it's key store extention usage as Server Authentication
        /// </summary>
        /// <param name="certThumbprint"></param>
        /// <returns></returns>
        Task<bool> SetAuthKeyUsageExtension(string certThumbprint);
    }
}
