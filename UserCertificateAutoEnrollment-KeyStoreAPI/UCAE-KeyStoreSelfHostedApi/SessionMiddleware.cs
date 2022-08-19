using Microsoft.Extensions.Primitives;
using System.Net;
using UserCertificateAutoEnrollment.BL.Session;

namespace UCAE_KeyStoreSelfHostedApi
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ISessionProvider sessionProvider)
        {
            StringValues value;
            httpContext.Request.Headers.TryGetValue("SessionKey", out value);
            var sessionKey = value.FirstOrDefault();

            try
            {
                var session = sessionProvider.GetSession(sessionKey);

                if (session == null)
                {
                    await HandleSessionMiddlewareError(httpContext, $"Can't do request, invalid {nameof(sessionKey)}");

                    return;
                }
            }
            catch (Exception ex)
            {
                await HandleSessionMiddlewareError(httpContext, ex.Message);

                return;
            }


            await _next(httpContext);
        }

        private async Task HandleSessionMiddlewareError(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(message);
            
            await context.Response.StartAsync();
        }
    }
}
