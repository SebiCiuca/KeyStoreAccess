namespace UserCertificateAutoEnrollment.BL.Http
{
    public  interface IHttpClient
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

    }
}
