using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserCertificateAutoEnrollment.BL.Common;

namespace UserCertificateAutoEnrollment_KeyStoreAPI
{
    public static class Config
    {
        public static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();

            services.AddSingleton(config);

            services.ConfigureKeyStoreServices();
            
            services.AddTransient<App>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
