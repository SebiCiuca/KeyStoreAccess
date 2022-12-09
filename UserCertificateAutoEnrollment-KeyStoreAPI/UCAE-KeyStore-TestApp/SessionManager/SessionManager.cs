using Sentry;

namespace UCAE_KeyStore_TestApp.SessionManager
{
    public class SessionManager : ISessionManager
    {
        private readonly object m_Lock = new object();
        private ITransaction m_Transaction;
        private ISpan m_Span;
        private string m_Key;

        private ITransaction GenerateNewTransaction(string sessionKey, string operation)
        {
            m_Key = "UCAEHost-UnknownSession";
            if (!string.IsNullOrEmpty(sessionKey))
            {
                m_Key = sessionKey;
            }

            m_Key = sessionKey;

            if (m_Transaction != null)
            {
                m_Transaction.Finish();
            }

            return SentrySdk.StartTransaction(m_Key, operation);

        }

        public Sentry.ISpan GetTransaction(string sessionKey, int commandId)
        {
            lock (m_Lock)
            {
                if (m_Transaction == null)
                {
                    m_Transaction = GenerateNewTransaction(sessionKey, commandId.ToString());
                }
                else
                {
                    if (m_Key != sessionKey)
                    {
                        m_Transaction = GenerateNewTransaction(sessionKey, commandId.ToString());
                    }
                }

                SentrySdk.ConfigureScope(scope => scope.Transaction = m_Transaction);

                m_Span = m_Transaction.StartChild(commandId.ToString());

                return m_Span;
            }
        }

        public void FinishTransaction()
        {
            m_Transaction?.Finish();

            m_Transaction = null;
        }
    }
}
