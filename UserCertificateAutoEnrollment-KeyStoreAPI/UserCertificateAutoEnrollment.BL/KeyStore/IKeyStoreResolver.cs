using System.Security.Cryptography.X509Certificates;

namespace UserCertificateAutoEnrollment.BL.KeyStore
{
    public interface IKeyStoreResolver
    {
        /// <summary>
        /// List certificates saved on client machine
        /// </summary>
        /// <returns></returns>
        IEnumerable<CertificateModel> ListKeys();
        /// <summary>
        /// Adds a new certificate (P12) to the client machine
        /// </summary>
        void AddCertificate(string keyName, string certificateName);
        /// <summary>
        /// Add a certifcate to a specific store and location
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="certificateName"></param>
        void AddCertificate(StoreName storeName, X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine);
        /// <summary>
        /// Change extended key usage.
        /// </summary>
        void SetKeyUsage(string certificateName, List<OidsModel> extenedKeyUsage);
        /// <summary>
        /// Delete a certificate from client machine
        /// </summary>
        void DeleteCertficate(string certificateName);
        bool ValidateCertificate(byte[] codeSignCertBytes, byte[] condeSignCertIssuerBytes, byte[] rca);
    }
}
