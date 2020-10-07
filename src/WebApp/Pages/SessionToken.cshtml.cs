using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluffyBunny.OAuth2TokenManagment;
using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApp.Services;

namespace WebApp.Pages
{
    public class SessionTokenModel : PageModel
    {
        string GuidS => Guid.NewGuid().ToString();

        public Dictionary<string, string> TokenReponse { get; private set; }
        public Dictionary<string, string> ManagedToken { get; private set; }

        private ISessionOAuth2Introspection _sessionOAuth2Introspection;
        private ITokenManager<SessionTokenStorage> _sessionTokenManager;
        private IFakeTokenFetchService _fakeTokenFetchService;
        private ILogger<SessionTokenModel> _logger;
        public SessionTokenModel(
            ITokenManager<SessionTokenStorage> sessionTokenManager,
            IFakeTokenFetchService fakeTokenFetchService,
            ISessionOAuth2Introspection sessionOAuth2Introspection,
            ILogger<SessionTokenModel> logger)
        {
            _sessionOAuth2Introspection = sessionOAuth2Introspection;
            _sessionTokenManager = sessionTokenManager;
            _fakeTokenFetchService = fakeTokenFetchService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var tokenResponse = await _fakeTokenFetchService.GetTokenExchangeAsync(GuidS);
            TokenReponse = tokenResponse.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(tokenResponse, null).ToString());
            await _sessionTokenManager.AddManagedTokenAsync("fake", new ManagedToken
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                CredentialsKey = "fake"
            });

            // This only gets put in so it can be checked for revocation in a middleware.
            await _sessionOAuth2Introspection.AddManagedTokenAsync(new ManagedToken
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                CredentialsKey = "fake"
            });
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var managedToken = await _sessionTokenManager.GetManagedTokenAsync("fake");
            ManagedToken = managedToken.GetType()
           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .ToDictionary(prop => prop.Name, prop =>
               {
                   var value = prop.GetValue(managedToken, null);
                   if (value == null)
                   {
                       return null;
                   }
                   else
                   {
                       return value.ToString();
                   }
               });

            return Page();
        }
    }
}
