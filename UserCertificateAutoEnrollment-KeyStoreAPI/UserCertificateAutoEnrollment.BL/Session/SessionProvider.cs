using System.Net.Http;
using System.Text;
using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCertificateAutoEnrollment.BL.Helpers;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Security;

namespace UserCertificateAutoEnrollment.BL.Session
{
    public class SessionProvider : ISessionProvider
    {
        private readonly ICryptoService m_CryptoService;
        private Dictionary<byte[], ISession> m_Sessions;

        public SessionProvider(ICryptoService cryptoService)
        {
            m_CryptoService = cryptoService;
            m_Sessions = new(new ByteArrayComparer());
        }

        public ISession CurrentSession => new Session(new byte[10]);
        public byte[] CodeSignCertificate { get; private set; }

        public async Task<ISession> CreateSession(string nonceValue)
        {
            System.Diagnostics.Debug.WriteLine("Starting Session");
            System.Diagnostics.Debug.WriteLine("Retriving nonce value from PKI");

            var generatedEncrpyedPassword = m_CryptoService.EncrpyRandomPassword(nonceValue);

            if (null == generatedEncrpyedPassword)
            {
                throw new Exception("Could not generate random password for session");
            }

            var session = new Session(generatedEncrpyedPassword);

            m_Sessions.Add(generatedEncrpyedPassword, session);

            return session;
        }

        public ISession GetSession(byte[] sessionKey)
        {
            if (null == sessionKey)
            {
                throw new ArgumentException($"'{nameof(sessionKey)}' cannot be null or whitespace.", nameof(sessionKey));
            }

            var sessionFound = m_Sessions.TryGetValue(sessionKey, out var session);

            if (!sessionFound)
            {
                throw new Exception("Session not found");
            }

            //CurrentSession = session;

            return session;
        }

        public ISession GetSession(string sessionKey)
        {
            if (null == sessionKey)
            {
                throw new ArgumentException($"'{nameof(sessionKey)}' cannot be null or whitespace.", nameof(sessionKey));
            }

            byte[] seesionKeyByte = Convert.FromBase64String(sessionKey);
            var sessionFound = m_Sessions.TryGetValue(seesionKeyByte, out var session);

            if (!sessionFound)
            {
                return null;
            }

            //CurrentSession = session;

            return session;
        }

        public void RemoveSession(byte[] sessionKey)
        {
            if (null == sessionKey)
            {
                throw new ArgumentException($"'{nameof(sessionKey)}' cannot be null or whitespace.", nameof(sessionKey));
            }

            var sessionFound = m_Sessions.TryGetValue(sessionKey, out var session);

            if (sessionFound)
            {
                if (session.Equals(CurrentSession))
                {
                    //CurrentSession = null;
                }

                m_Sessions.Remove(sessionKey);
            }
        }

        public bool ValidateSession(byte[] sessionKey)
        {
            if (null == sessionKey)
            {
                throw new ArgumentException($"'{nameof(sessionKey)}' cannot be null or whitespace.", nameof(sessionKey));
            }

            var sessionFound = m_Sessions.TryGetValue(sessionKey, out var session);

            if (!sessionFound)
            {
                return false;
            }

            session.ValidateSession();

            return true;
        }
    }
}
