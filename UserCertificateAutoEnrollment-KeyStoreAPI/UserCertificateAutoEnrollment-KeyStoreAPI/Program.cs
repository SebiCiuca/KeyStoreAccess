// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using UserCertificateAutoEnrollment_KeyStoreAPI;


var services = Config.ConfigureServices();

var serviceProvider = services.BuildServiceProvider();

serviceProvider.GetService<App>()
    .RunAsync();


