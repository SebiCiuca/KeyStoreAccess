namespace UserCertificateAutoEnrollment.BL.Common.Contracts
{
    public interface IHttpClient
    {
        /// <summary>
        /// Calls server to get a nonce value to start a new client session
        /// </summary>
        /// <returns>Nonce value</returns>
        Task<string> GetNonceValue();

        /// <summary>
        /// Validates a session password created using nonce value retrieved
        /// with the server
        /// </summary>
        Task<bool> ValidateSessionPassword(byte[] password);

        /// <summary>
        /// Based on a given link, download a certifcates as byte[]
        /// </summary>
        /// <param name="url">Url to certificate byte</param>
        /// <returns></returns>
        Task<byte[]> GetCertificate(string url);

        /// <summary>
        /// Based on a given link (be it certificate or signature link) download certificate signature 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<byte[]> GetCertificateSignature(string url);

    }
}
