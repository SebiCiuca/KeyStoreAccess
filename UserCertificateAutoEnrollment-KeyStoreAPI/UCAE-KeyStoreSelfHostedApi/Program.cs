using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.WindowsServices;
using UCAE_KeyStoreSelfHostedApi;
using UserCertificateAutoEnrollment.BL.Security;
using UserCertificateAutoEnrollment.BL.Session;

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

builder.Services.AddSingleton<ISessionProvider, SessionProvider>();
builder.Services.AddTransient<ICryptoService, CryptoService>();

builder.Host.UseWindowsService();


var app = builder.Build();

//app.UseMyCustomMiddleware();

app.UseWhen(c => c.Request.Path.StartsWithSegments("/WeatherForecast", StringComparison.OrdinalIgnoreCase),
    appBuilder =>
    {
        appBuilder.UseMyCustomMiddleware();
    });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
