using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluffyBunny.OAuth2TokenManagment;
using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class OAuth2ManagementModel : PageModel
    {
        private IOAuth2CredentialManager _oAuth2CredentialManager;
        private ITokenManager<GlobalDistributedCacheTokenStorage> _globalTokenManager;

        public OAuth2ManagementModel(
            IOAuth2CredentialManager oAuth2CredentialManager,
            ITokenManager<GlobalDistributedCacheTokenStorage> globalTokenManager)
        {
            _oAuth2CredentialManager = oAuth2CredentialManager;
            _globalTokenManager = globalTokenManager;
        }

        public OAuth2Credentials Creds { get; private set; }
        public ManagedToken ManagedToken { get; private set; }

        public async Task OnGetAsync()
        {
            Creds = await _oAuth2CredentialManager.GetOAuth2CredentialsAsync("test");
            ManagedToken = await _globalTokenManager.GetManagedTokenAsync("test", true);
        }
    }
}
