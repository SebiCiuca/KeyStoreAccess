namespace UserCertificateAutoEnrollment.BL.Session
{
    public interface ISessionProvider
    {
        Task<ISession> CreateSession(string nonceValue);
        ISession GetSession(byte[] sessionKey);
        ISession GetSession(string sessionKey);
        void ValidateSession(byte[] sessionKey);
        void RemoveSession(byte[] sessionKey);
    }
}
