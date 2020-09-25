using FluffyBunny.OAuth2TokenManagment.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface ICustomTokenRequestManager
    {
        void AddTokenRequestFunction(string key, Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>> func);
        Func<ManagedToken, IServiceProvider, IOAuth2CredentialManager, CancellationToken, Task<ManagedToken>> GetTokenRequestFunc(string key);
    }
}
