using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserCertificateAutoEnrollment.BL.Common;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCetrificateAutoEnrollment.BL.Windows;

namespace UserCertificateAutoEnrollment.BL
{
    public static class ConfigurationExtentions
    {
        public static void ConfigureKeyStoreServices(this IServiceCollection services)
        {
            services.AddTransient<IKeyStoreFactory, KeyStoreFactory>();
        }

        public static void ConfigureAppSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Http.HttpClientUrls urls = new();

            configuration.GetSection("HttpClientUrls").Bind(urls);

            //Create singleton from instance
            serviceCollection.AddSingleton(urls);
        }
    }
}
