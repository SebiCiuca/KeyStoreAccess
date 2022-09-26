using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Security;
using UserCertificateAutoEnrollment.BL.Session;

namespace UCAE_KeyStore.Helpers
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
                     loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                     loggingBuilder.AddNLog();
                 })     
                .AddSingleton<IMessageProcessor, MessageProcessor>()
                .AddTransient<IKeyStoreFactory, KeyStoreFactory>()
                .AddSingleton<ISessionProvider, SessionProvider>()
                .AddTransient<ICryptoService, CryptoService>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
