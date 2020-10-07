using Microsoft.AspNetCore.Builder;

namespace FluffyBunny.OAuth2TokenManagment.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSessionOAuth2Introspection(this IApplicationBuilder app)
        {
            app.UseMiddleware<SessionOAuth2IntrospectionMiddleware>();
            return app;
        }
    }
}
