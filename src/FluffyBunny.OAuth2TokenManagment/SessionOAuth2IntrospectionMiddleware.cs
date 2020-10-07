using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment
{
    public class SessionOAuth2IntrospectionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly SessionOAuth2IntrospectionOptions _options;
        private readonly ILogger<SessionOAuth2IntrospectionMiddleware> _logger;

        public SessionOAuth2IntrospectionMiddleware(
            RequestDelegate next, 
            IOptions<SessionOAuth2IntrospectionOptions> options,
            ILogger<SessionOAuth2IntrospectionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, 
            ISessionOAuth2Introspection sessionOAuth2Introspection,
            ITokenManager<SessionTokenStorage> sessionTokenManager,
            IServiceProvider serviceProvider
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.User.Identity.IsAuthenticated &&
                sessionOAuth2Introspection.IsActive)
            {
                var key = sessionOAuth2Introspection.ManagedTokenKey;
                _logger.LogInformation($"SessionOAuth2IntrospectionMiddleware:{key}");
                var managedToken = await sessionTokenManager.GetManagedTokenAsync(key);
                if(managedToken == null)
                {
                    var signinManager = serviceProvider.GetRequiredService<ISigninManager>();
                    await signinManager.SignOutAsync();
                    context.Session.Clear();
                    context.Response.Redirect(_options.RedirectUrl);
                    return;
                }

            }
            await _next(context);
        }
    }
}
