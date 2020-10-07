using FluffyBunny.OAuth2TokenManagment.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface ISessionOAuth2Introspection
    {
        Task AddManagedTokenAsync(ManagedToken managedToken);
        string ManagedTokenKey { get; }
        bool IsActive { get; }
    }
    public class SessionOAuth2Introspection : ISessionOAuth2Introspection
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenManager<SessionTokenStorage> _sessionTokenManager;

        public SessionOAuth2Introspection(
            IHttpContextAccessor httpContextAccessor,
            ITokenManager<SessionTokenStorage> sessionTokenManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionTokenManager = sessionTokenManager;
        }

        public string ManagedTokenKey { get { return "4a959892-cd87-46cf-9ac5-e6c5802a4cc7"; } }

        public bool IsActive
        {
            get
            {
                byte[] value;
                bool exists = _httpContextAccessor.HttpContext.Session.TryGetValue(ManagedTokenKey, out value);
                return exists;
            }
        }


        public async Task AddManagedTokenAsync(ManagedToken managedToken)
        {
            await _sessionTokenManager.AddManagedTokenAsync(ManagedTokenKey, managedToken);
            _httpContextAccessor.HttpContext.Session.SetInt32(ManagedTokenKey, 1); // hint that we are tracking
        }
    }
}

