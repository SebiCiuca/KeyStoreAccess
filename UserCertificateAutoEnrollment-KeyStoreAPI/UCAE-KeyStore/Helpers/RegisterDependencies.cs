using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Sentry;
using UCAE_KeyStore.SessionManager;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Security;
using UserCertificateAutoEnrollment.BL.Session;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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
                     loggingBuilder.AddSentry(config =>
                     {
                         config.Dsn = "https://1dbc3eb561854286a6932798932315c3@o4503936065732608.ingest.sentry.io/4503936067829760";
                         config.InitializeSdk = false;
                     });
                 })
                .AddSingleton<IMessageProcessor, MessageProcessor>()
                .AddTransient<IKeyStoreFactory, KeyStoreFactory>()
                .AddSingleton<ISessionManager, SessionManager.SessionManager>()
                .AddTransient<ICryptoService, CryptoService>()
                .BuildServiceProvider();


            SentrySdk.Init(o =>
            {
                // NOTE: Change the URL below to your own DSN. Get it on sentry.io in the project settings (or when you create a new project):
                o.Dsn = "https://1dbc3eb561854286a6932798932315c3@o4503936065732608.ingest.sentry.io/4503936067829760";

                o.TracesSampleRate = 1.0;
                // Enable offline caching
                o.CacheDirectoryPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "SentryCache"
                );
            });

            return serviceProvider;
        }
    }
}
