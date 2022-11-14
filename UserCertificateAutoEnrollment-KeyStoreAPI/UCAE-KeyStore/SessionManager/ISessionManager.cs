using Sentry;

namespace UCAE_KeyStore.SessionManager
{
    public interface ISessionManager
    {
        ISpan GetTransaction(string sessionKey, int commandId);
        void FinishTransaction();
    }
}
