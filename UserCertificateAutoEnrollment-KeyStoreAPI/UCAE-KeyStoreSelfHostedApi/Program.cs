using Microsoft.Extensions.Hosting.WindowsServices;
using NLog;
using NLog.Web;
using UCAE_KeyStoreSelfHostedApi;
using UserCertificateAutoEnrollment.BL;
using UserCertificateAutoEnrollment.BL.Common.Contracts;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Security;
using UserCertificateAutoEnrollment.BL.Session;

var logger = NLog.LogManager.Setup().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var options = new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
    };

    var builder = WebApplication.CreateBuilder(options);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<ISessionProvider, SessionManager>();
    builder.Services.AddTransient<ICryptoService, CryptoService>();
    builder.Services.AddTransient<IHttpClient, UserCertificateAutoEnrollment.BL.Http.HttpClient>();
    builder.Services.ConfigureKeyStoreServices();

    builder.Host.UseWindowsService();
    builder.Host.UseNLog();

    var app = builder.Build();

    //app.UseMyCustomMiddleware();

    //app.UseWhen(c => c.Request.Path.StartsWithSegments("/WeatherForecast", StringComparison.OrdinalIgnoreCase),
    //    appBuilder =>
    //    {
    //        appBuilder.UseMyCustomMiddleware();
    //    });

    //app.UseWhen(c => c.Request.Path.StartsWithSegments("/KeyStore", StringComparison.OrdinalIgnoreCase),
    //    appBuilder =>
    //    {
    //        appBuilder.UseMyCustomMiddleware();
    //    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}