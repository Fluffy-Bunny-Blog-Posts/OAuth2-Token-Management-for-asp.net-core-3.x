using FluffyBunny.OAuth2TokenManagment.Models;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface IOAuth2CredentialManager
    {
        Task<OAuth2Credentials> GetOAuth2CredentialsAsync(string key);
        Task AddCredentialsAsync(string key, OAuth2Credentials creds);
    }
}
