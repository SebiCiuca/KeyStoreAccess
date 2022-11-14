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

        ///// <summary>
        ///// Verify certificate and it signature
        ///// </summary>
        ///// <param name="certificateBytes"></param>
        ///// <param name="certifcatesBytesSignature"></param>
        //bool VerifySignature(byte[] certificateBytes, byte[] certifcatesBytesSignature);

        ///// <summary>
        ///// Adds a new certificate (P12) to the client machine
        ///// </summary>
        ///// <summary>
        ///// Add a certifcate to a specific store and location
        ///// </summary>
        ///// <param name="keyName"></param>
        ///// <param name="certificateName"></param>
        ////void AddCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine);
        /////// <summary>
        /////// Change extended key usage.
        /////// </summary>
        ////void SetKeyUsage(string certificateName, List<OidsModel> extenedKeyUsage);
        /////// <summary>
        /////// Delete a certificate from client machine
        /////// </summary>
        ////void DeleteCertficate(string certificateName);

        /////Validate CodeSign Certificate, this will be used to validate further certificates
        ////bool ValidateCertificate(byte[] codeSignCertBytes, byte[] condeSignCertIssuerBytes, byte[] rca);


        //Task<bool> InitializeTrust();
        //Task<bool> ImportCertificate(byte[] certificateByte, string sSTType);
    }
}
