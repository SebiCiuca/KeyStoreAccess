using Sentry;

namespace UCAE_KeyStore_TestApp.SessionManager
{
    public interface ISessionManager
    {
        ISpan GetTransaction(string sessionKey, int commandId);
        void FinishTransaction();
    }
}
