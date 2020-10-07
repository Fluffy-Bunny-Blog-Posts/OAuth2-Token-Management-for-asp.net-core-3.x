using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    public class DefaultSigninManager : ISigninManager
    {
        private SignInManager<IdentityUser> _signinManager;

        public DefaultSigninManager(SignInManager<IdentityUser> signinManager)
        {
            _signinManager = signinManager;
        }
        public Task SignOutAsync()
        {
            return _signinManager.SignOutAsync();
        }
    }
}
