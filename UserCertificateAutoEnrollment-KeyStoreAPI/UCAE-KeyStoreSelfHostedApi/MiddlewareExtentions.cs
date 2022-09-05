namespace UCAE_KeyStoreSelfHostedApi
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseMyCustomMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionMiddleware>();

        }
    }
}
