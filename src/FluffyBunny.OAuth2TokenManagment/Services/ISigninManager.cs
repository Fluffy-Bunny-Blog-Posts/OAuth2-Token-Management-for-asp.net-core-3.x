using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface ISigninManager
    {
        Task SignOutAsync();
    }
}
