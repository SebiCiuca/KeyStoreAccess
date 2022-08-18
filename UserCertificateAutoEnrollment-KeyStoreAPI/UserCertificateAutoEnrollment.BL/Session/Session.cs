using System.Text;

namespace UserCertificateAutoEnrollment.BL.Session
{
    public class Session : ISession
    {
        private byte[] m_SessionKey;
        private bool m_IsValid;

        public byte[] SessionKey => m_SessionKey;

        public bool IsValid => m_IsValid;

        public string SessionKeyASCI => Encoding.ASCII.GetString(m_SessionKey);

        public string SessionKeyUTF8 => Encoding.UTF8.GetString(m_SessionKey);
        public string SessionKeyBaseC6 => Convert.ToBase64String(m_SessionKey);

        public Session(byte[] sessionKey)
        {
            m_SessionKey = sessionKey ?? throw new ArgumentNullException(nameof(sessionKey));
            m_IsValid = false;
        }

        public void ValidateSession()
        {
            m_IsValid = true;
        }
    }
}
