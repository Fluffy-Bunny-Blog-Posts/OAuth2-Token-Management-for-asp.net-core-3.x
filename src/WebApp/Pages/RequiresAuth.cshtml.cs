using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluffyBunny.OAuth2TokenManagment;
using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using WebApp.Services;

namespace WebApp.Pages
{
    [Authorize]
    public class RequiresAuthModel : PageModel
    {
        string GuidS => Guid.NewGuid().ToString();

        public Dictionary<string, string> ManagedToken { get; private set; }

        private ITokenManager<SessionTokenStorage> _sessionTokenManager;
        private ILogger<RequiresAuthModel> _logger;

        public RequiresAuthModel(
            ITokenManager<SessionTokenStorage> sessionTokenManager,
            ILogger<RequiresAuthModel> logger)
        {
            _sessionTokenManager = sessionTokenManager;
            _logger = logger;
        }
        public async Task OnGetAsync()
        {
            var managedToken = await _sessionTokenManager.GetManagedTokenAsync("fake");
            if(managedToken != null)
            {
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
            }
        }
    }
}
