using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluffyBunny.OAuth2TokenManagment.Services;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{

    public class CustomTokenRequestManager : ICustomTokenRequestManager
    {
        Dictionary<string,
            Func<ManagedToken,
                IServiceProvider,
                IOAuth2CredentialManager,
                CancellationToken,
                Task<ManagedToken>>> _functions;
        public CustomTokenRequestManager(ILogger<CustomTokenRequestManager> logger)
        {
            _functions = new Dictionary<string, Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>>>
            {
                { "client_credentials", ExecuteClientCredentialsRequestAsync }
            };

        }
        static async Task<ManagedToken> ExecuteClientCredentialsRequestAsync(
           ManagedToken managedToken,
           IServiceProvider serviceProvider,
           IOAuth2CredentialManager oAuth2CredentialManager,
           CancellationToken cancellationToken = default)
        {
            var creds = await oAuth2CredentialManager.GetOAuth2CredentialsAsync(managedToken.CredentialsKey);
            var client = new HttpClient();
            var response = await client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = creds.DiscoveryDocumentResponse.TokenEndpoint,
                    ClientId = creds.ClientId,
                    ClientSecret = creds.ClientSecret,
                    Scope = managedToken.RequestedScope
                },
                cancellationToken);
            if (response.IsError)
            {
                throw new Exception(response.Error);
            }
            managedToken.RefreshToken = response.RefreshToken;
            managedToken.AccessToken = response.AccessToken;
            managedToken.ExpiresIn = response.ExpiresIn;
            return managedToken;
        }
        public void AddTokenRequestFunction(string key, Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>> func)
        {
            _functions.Add(key, func);
        }

        public Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>> GetTokenRequestFunc(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>> func;
            _functions.TryGetValue(key, out func);
            return func;
        }
    }
}
