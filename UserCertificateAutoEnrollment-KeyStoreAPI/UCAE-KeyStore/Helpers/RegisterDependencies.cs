using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
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
                 })
                .AddSingleton<IMessageProcessor, MessageProcessor>()
                .AddTransient<IKeyStoreFactory, KeyStoreFactory>()
                .AddSingleton<ISessionProvider, SessionProvider>()
                .AddTransient<ICryptoService, CryptoService>()
                .BuildServiceProvider();

            //LogManager.Configuration.AddSentry(o =>
            //{
            //    o.Dsn = "https://1dbc3eb561854286a6932798932315c3@o4503936065732608.ingest.sentry.io/4503936067829760";
            //    o.Debug = true;
            //    o.TracesSampleRate = 1.0;
            //    o.IsGlobalModeEnabled = true;
            //    // Optionally specify a separate format for message
            //    o.Layout = "${message}";
            //    // Optionally specify a separate format for breadcrumbs
            //    o.BreadcrumbLayout = "${logger}: ${message}";

            //    // Debug and higher are stored as breadcrumbs (default is Info)
            //    o.MinimumBreadcrumbLevel = NLog.LogLevel.Debug;
            //    // Error and higher is sent as event (default is Error)
            //    o.MinimumEventLevel = NLog.LogLevel.Error;

            //    // Send the logger name as a tag
            //    o.AddTag("logger", "${logger}");

            //    // All Sentry Options are accessible here.
            //});

            return serviceProvider;
        }
    }
}
