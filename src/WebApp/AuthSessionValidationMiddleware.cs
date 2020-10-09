using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace WebApp
{
    public class AuthSessionValidationMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly AuthSessionValidationOptions _options;
        private readonly ILogger<AuthSessionValidationMiddleware> _logger;

        public AuthSessionValidationMiddleware(
            RequestDelegate next,
            IOptions<AuthSessionValidationOptions> options,
            ILogger<AuthSessionValidationMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context,IServiceProvider serviceProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.User.Identity.IsAuthenticated)
            {
                var ttt = context.Session.GetString("ttt");
                if (ttt == null)
                {
                    var signinManager = serviceProvider.GetRequiredService<ISigninManager>();
                    await signinManager.SignOutAsync();
                    context.Session.Clear();
                    context.Response.Redirect(_options.RedirectUrl);
                    _logger.LogError($"Auth Session Requirements not met");
                    return;
                }
              
            }
            await _next(context);
        }
    }
   
}
