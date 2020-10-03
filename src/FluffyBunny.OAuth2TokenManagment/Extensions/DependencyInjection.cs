using FluffyBunny.OAuth2TokenManagment.Services;
using FluffyBunny.OAuth2TokenManagment.Services.Default;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FluffyBunny.OAuth2TokenManagment.Extensions
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddManagedTokenServices(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer, Serializer>();
            services.AddSingleton<IDataProtectorAccessor, DataProtectorAccessor>();
            services.AddSingleton<IOpenIdConnectConfigurationAccessor, OpenIdConnectConfigurationAccessor>();
            services.AddSingleton<ICustomTokenRequestManager, CustomTokenRequestManager>();
            services.AddScoped<GlobalDistributedCacheTokenStorage>();
            services.AddScoped<SessionTokenStorage>();
            services.AddScoped(typeof(ITokenManager<>), typeof(TokenManager<>));
            services.AddScoped<IOAuth2CredentialManager, OAuth2CredentialManager>();
            return services;
        }
    }
}
