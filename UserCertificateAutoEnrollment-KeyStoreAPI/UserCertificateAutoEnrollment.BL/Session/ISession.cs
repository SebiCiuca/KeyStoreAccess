namespace UserCertificateAutoEnrollment.BL.Session
{
    public interface ISession
    {
        byte[] SessionKey { get; }
        string SessionKeyASCI { get; }
        string SessionKeyUTF8 { get; }
        string SessionKeyBaseC6 { get; }
        bool IsValid { get; }
        void ValidateSession();
    }
}
