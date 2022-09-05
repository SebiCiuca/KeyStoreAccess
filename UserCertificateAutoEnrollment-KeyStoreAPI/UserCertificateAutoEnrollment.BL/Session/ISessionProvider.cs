using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.Session
{
    public interface ISessionProvider
    {
        ISession CurrentSession { get; }
        Task<ISession> CreateSession(string nonceValue);
        ISession GetSession(byte[] sessionKey);
        ISession GetSession(string sessionKey);
        bool ValidateSession(byte[] sessionKey);
        void RemoveSession(byte[] sessionKey);
    }
}
