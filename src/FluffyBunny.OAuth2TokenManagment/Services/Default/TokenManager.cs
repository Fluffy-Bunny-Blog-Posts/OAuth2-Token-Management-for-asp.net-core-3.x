using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static FluffyBunny.OAuth2TokenManagment.TimedLock;
using FluffyBunny.OAuth2TokenManagment.Services;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    /*
     We don't persist credentials to anywhere, we will keep our OAuth2 credentials in IMemoryCache and our 
    "so-called" client_id/client_secrets will be references to the actual credentials.

    each service will have their own in-memory copy
     */
    public class TokenManager<T> : ITokenManager<T> where T : TokenStorage
    {
        static TimedLock _lock = new TimedLock();
        private IHttpClientFactory _clientFactory;
        private IServiceProvider _serviceProvider;
        private IMemoryCache _memoryCache;
        private IOAuth2CredentialManager _oAuth2CredentialManager;
        private T _tokenStorage;
        private ICustomTokenRequestManager _customTokenRequest;
        private ILogger<TokenManager<T>> _logger;


        public TokenManager(
            IHttpClientFactory clientFactory,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IOAuth2CredentialManager oAuth2CredentialManager,
            T tokenStorage,
            ICustomTokenRequestManager customTokenRequest,
            ILogger<TokenManager<T>> logger)
        {
            _clientFactory = clientFactory;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _oAuth2CredentialManager = oAuth2CredentialManager;
            _tokenStorage = tokenStorage;
            _customTokenRequest = customTokenRequest;
            _logger = logger;
        }

        private async Task<ManagedToken> AddUnSafeManagedTokenAsync(string key, ManagedToken managedToken)
        {
            managedToken.StartDate = DateTimeOffset.UtcNow;
            managedToken.ExpirationDate = managedToken.StartDate.AddSeconds(managedToken.ExpiresIn);
            await _tokenStorage.UpsertManagedTokenAsync(key, managedToken);
            return managedToken;
        }
        private async Task<ManagedToken> GetUnSafeManagedTokenAsync(string key)
        {
            ManagedToken managedToken = await _tokenStorage.GetManagedTokenAsync(key);
            return managedToken;
        }

        public async Task<ManagedToken> AddManagedTokenAsync(string key, ManagedToken managedToken)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                return await AddUnSafeManagedTokenAsync(key, managedToken);
            }
            finally
            {
                releaser.Dispose();
            }
        }

        public async Task RemoveManagedTokenAsync(string key)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                await _tokenStorage.RemoveManagedTokenAsync(key);
            }
            finally
            {
                releaser.Dispose();
            }
        }
        public async Task<ManagedToken> GetManagedTokenAsync(string key, bool forceRefresh = false, CancellationToken cancellationToken = default)
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                var managedToken = await GetUnSafeManagedTokenAsync(key);
                if (managedToken == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(managedToken.RefreshToken))
                {
                    if (forceRefresh)
                    {
                        var func = _customTokenRequest.GetTokenRequestFunc(managedToken.RequestFunctionKey);
                        if (func == null)
                        {
                            throw new Exception($"forceRefresh requested, but no token request function exists. RequestFunctionKey={managedToken.RequestFunctionKey}");
                        }
                        var mT = await func(managedToken, _serviceProvider, _oAuth2CredentialManager, cancellationToken);
                        if (mT == null)
                        {
                            throw new Exception($"Custom Token Request function return a null. RequestFunctionKey={managedToken.RequestFunctionKey}");
                        }
                        managedToken = await AddUnSafeManagedTokenAsync(key, mT);

                    }
                    return managedToken;
                }

                DateTimeOffset now = DateTimeOffset.UtcNow;
                if (!forceRefresh)
                {
                    if (managedToken.ExpirationDate > now || string.IsNullOrWhiteSpace(managedToken.RefreshToken))
                    {
                        return managedToken;
                    }
                }
                var creds = await _oAuth2CredentialManager.GetOAuth2CredentialsAsync(managedToken.CredentialsKey);
                if (creds == null)
                {
                    throw new Exception($"GetOAuth2CredentialsAsync failed: key={managedToken.CredentialsKey}");
                }
                HttpClient client = null;
                if (string.IsNullOrEmpty(creds.HttpClientName))
                {
                    client = new HttpClient();
                }
                else
                {
                    client = _clientFactory.CreateClient(creds.HttpClientName);
                }

               
                var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = creds.DiscoveryDocumentResponse.TokenEndpoint,
                    ClientId = creds.ClientId,
                    ClientSecret = creds.ClientSecret,
                    RefreshToken = managedToken.RefreshToken
                });
                if (response.IsError)
                {
                    // REMOVE the managed token, because the refresh failed
                    await _tokenStorage.RemoveManagedTokensAsync();
                    throw new Exception($"RequestRefreshTokenAsync failed for key={key}",
                        new Exception(response.Error));
                }
                managedToken.RefreshToken = response.RefreshToken;
                managedToken.AccessToken = response.AccessToken;
                managedToken.ExpiresIn = response.ExpiresIn;
                managedToken = await AddUnSafeManagedTokenAsync(key, managedToken);
                return managedToken;
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

        public async Task RemoveAllManagedTokenAsync()
        {
            LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
            try
            {
                await _tokenStorage.RemoveManagedTokensAsync();
            }
            finally
            {
                releaser.Dispose();
            }
        }

    }
}
