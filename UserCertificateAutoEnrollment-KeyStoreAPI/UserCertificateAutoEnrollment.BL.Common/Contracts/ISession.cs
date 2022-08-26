namespace UserCertificateAutoEnrollment.BL.Common.Contracts
{
    public interface ISession
    {
        byte[] SessionKey { get; }
        byte[] TrustCert { get; }
        bool IsValid { get; }
        void ValidateSession();
        void InializeTrustCert(byte[] trustBytes);
    }
}
