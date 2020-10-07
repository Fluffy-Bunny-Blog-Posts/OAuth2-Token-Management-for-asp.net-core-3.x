using FluffyBunny.OAuth2TokenManagment.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface ITokenManager<T> where T : TokenStorage
    {
        Task<ManagedToken> AddManagedTokenAsync(string key, ManagedToken tokenConfig);
        Task RemoveManagedTokenAsync(string key);
        Task<ManagedToken> GetManagedTokenAsync(string key, bool forceRefresh = false, CancellationToken cancellationToken = default);
        Task RemoveAllManagedTokenAsync();
    }
}
