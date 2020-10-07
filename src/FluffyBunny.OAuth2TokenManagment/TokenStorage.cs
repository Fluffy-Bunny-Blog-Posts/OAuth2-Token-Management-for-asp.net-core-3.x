using FluffyBunny.OAuth2TokenManagment.Models;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment
{
    public abstract class TokenStorage
    {
        public abstract Task<ManagedToken> GetManagedTokenAsync(string key);
        public abstract Task RemoveManagedTokenAsync(string key);
        public abstract Task UpsertManagedTokenAsync(string key, ManagedToken managedToken);
        public abstract Task RemoveManagedTokensAsync();

    }
}
