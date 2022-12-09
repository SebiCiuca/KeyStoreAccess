using Microsoft.Extensions.DependencyInjection;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.KeyStore;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using UCAE_KeyStore_TestApp.SessionManager;

namespace UCAE_KeyStore_TestApp.Helpers
{
    public static class RegisterDependencies
    {
        public static IServiceProvider RegisterAppDependencies()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                 {
                     // configure Logging with NLog
                     loggingBuilder.ClearProviders();
                     loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                     loggingBuilder.AddNLog();
                 })
                .AddSingleton<IMessageProcessor, MessageProcessor>()
                .AddTransient<IKeyStoreFactory, KeyStoreFactory>()
                .AddSingleton<ISessionManager, SessionManager.SessionManager>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
