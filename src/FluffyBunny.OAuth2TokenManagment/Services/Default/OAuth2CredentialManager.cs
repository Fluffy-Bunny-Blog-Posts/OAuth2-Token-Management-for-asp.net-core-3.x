using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment.Services;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static FluffyBunny.OAuth2TokenManagment.TimedLock;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    public class OAuth2CredentialManager : IOAuth2CredentialManager
    {
        static TimedLock _lock = new TimedLock();
        const string PreKeyName = "753fe8f1-df85-4560-ab74-85c4151b5df0";
        private IHttpClientFactory _clientFactory;
        private IMemoryCache _memoryCache;
        private ILogger<OAuth2CredentialManager> _logger;

        public OAuth2CredentialManager(
               IHttpClientFactory clientFactory,
               IMemoryCache memoryCache,
               ILogger<OAuth2CredentialManager> logger)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task AddCredentialsAsync(string key, OAuth2Credentials creds)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                var memKey = $"{PreKeyName}.{key}";
                _memoryCache.Set(memKey, creds);
            }
            finally
            {
                releaser.Dispose();
            }
        }

        public async Task<OAuth2Credentials> GetOAuth2CredentialsAsync(string key)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                var memKey = $"{PreKeyName}.{key}";
                OAuth2Credentials creds;
                if (_memoryCache.TryGetValue(memKey, out creds))
                {
                   
                    if (creds.DiscoveryDocumentResponse == null)
                    {
                        // pull it.  
                        HttpClient client = null;
                        if (string.IsNullOrEmpty(creds.HttpClientName))
                        {
                            client = new HttpClient();
                        }
                        else
                        {
                            client = _clientFactory.CreateClient(creds.HttpClientName);
                        }
                        var disco = await client.GetDiscoveryDocumentAsync(creds.Authority);
                        if (disco.IsError)
                            throw new Exception(disco.Error);
                        creds.DiscoveryDocumentResponse = disco;
                        _memoryCache.Set(memKey, creds);
                    }
                    return creds;
                }
                _logger.LogError($"OAuth2 Credentials is not in cache: key={key}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            finally
            {
                releaser.Dispose();
            }
        }
    }
}
