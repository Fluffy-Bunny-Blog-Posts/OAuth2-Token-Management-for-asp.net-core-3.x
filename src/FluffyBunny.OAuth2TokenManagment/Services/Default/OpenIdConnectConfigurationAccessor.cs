using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading;
using System.Threading.Tasks;
using static FluffyBunny.OAuth2TokenManagment.TimedLock;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    public class OpenIdConnectConfigurationAccessor : IOpenIdConnectConfigurationAccessor
    {
        private IMemoryCache _cache;
        private ILogger<OpenIdConnectConfigurationAccessor> _logger;
        static TimedLock _lock = new TimedLock();
        public OpenIdConnectConfigurationAccessor(
            IMemoryCache cache,
            ILogger<OpenIdConnectConfigurationAccessor> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<OpenIdConnectConfiguration> GetOpenIdConnectConfigurationAsync(string issuer)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                OpenIdConnectConfiguration openIdConfig;
                if (_cache.TryGetValue(issuer, out openIdConfig))
                {
                    return openIdConfig;
                }
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{issuer}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                _cache.Set(issuer, openIdConfig, new TimeSpan(1, 0, 0, 0));  // cache for 1 day
                return openIdConfig;
            }
            finally
            {
                releaser.Dispose();
            }
        }
    }
}
