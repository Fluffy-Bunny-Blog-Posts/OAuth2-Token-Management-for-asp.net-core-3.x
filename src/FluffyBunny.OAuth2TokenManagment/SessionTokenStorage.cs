using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluffyBunny.OAuth2TokenManagment.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;

namespace FluffyBunny.OAuth2TokenManagment
{
    // Storing all the tokens in a single Dictionary so I can remove them all in one hit.
    // Scoped object, so once its read out of the Session it is cached in edge memory 
    // until the duration of the scope.
    public class SessionTokenStorage : TokenStorage
    {
        public static string GuidS => Guid.NewGuid().ToString();
        const string _sessionKey = "f84e517c-df38-4cfa-b8cb-92e65d962887";
        private IHttpContextAccessor _httpContextAccessor;

        public class SessionKey
        {
            public string Key { get; set; }
        }

        protected ISerializer _serializer;
        protected IDataProtectorAccessor _dataProtectorAccessor;
        protected ILogger<SessionTokenStorage> _logger;
        public SessionTokenStorage(
            ISerializer serializer,
            IHttpContextAccessor httpContextAccessor,
            IDataProtectorAccessor dataProtectorAccessor,
            ILogger<SessionTokenStorage> logger)
        {
            _serializer = serializer;
            _httpContextAccessor = httpContextAccessor;
            _dataProtectorAccessor = dataProtectorAccessor;
            _logger = logger;
        }
        ISession Session => _httpContextAccessor.HttpContext.Session;
        Dictionary<string, ManagedToken> ManagedTokens { get; set; }
        public string GetCacheKey()
        {
            var sessionKey = Session.Get<SessionKey>(_sessionKey);
            if (sessionKey == null)
            {
                sessionKey = new SessionKey
                {
                    Key = GuidS
                };
                Session.Set(_sessionKey, sessionKey);
            }
            return sessionKey.Key;
        }

        protected Dictionary<string, ManagedToken> GetManagedTokens()
        {
            if(ManagedTokens != null)
            {
                return ManagedTokens;
            }
            var cacheKey = GetCacheKey();
            var protector = _dataProtectorAccessor.GetProtector(cacheKey);

            var json = Session.Get<string>(cacheKey);

            Dictionary<string, ManagedToken> managedTokens = null;
            if (string.IsNullOrWhiteSpace(json))
            {
                ManagedTokens = new Dictionary<string, ManagedToken>();
                UpsertManagedTokens(managedTokens);
                return ManagedTokens;
            }
            json = protector.Unprotect(json);

            ManagedTokens = _serializer.Deserialize<Dictionary<string, ManagedToken>>(json);
            return ManagedTokens;
        }
        protected void UpsertManagedTokens(Dictionary<string, ManagedToken> managedTokens)
        {
            var cacheKey = GetCacheKey();
            var protector = _dataProtectorAccessor.GetProtector(cacheKey);

            var json = _serializer.Serialize(managedTokens);
            json = protector.Protect(json);
            Session.Set<string>(cacheKey,json);
            ManagedTokens = managedTokens;
        }

        public async override Task<ManagedToken> GetManagedTokenAsync(string key)
        {

            var tokens = GetManagedTokens();
            if (tokens == null)
            {
                return null;
            }
            ManagedToken token;
            if (tokens.TryGetValue(key, out token))
            {
                return token;
            }
            return null;
        }
        public async override Task RemoveManagedTokenAsync(string key)
        {
            var tokens = GetManagedTokens();
            if (tokens == null)
            {
                return;
            }
            if (tokens.ContainsKey(key))
            {
                tokens.Remove(key);
                UpsertManagedTokens(tokens);
            }
        }

        public async override Task UpsertManagedTokenAsync(string key, ManagedToken managedToken)
        {
            var tokens = GetManagedTokens();
            if (tokens == null)
            {
                tokens = new Dictionary<string, ManagedToken>();
            }
            tokens[key] = managedToken;
            UpsertManagedTokens(tokens);
        }

        public async override Task RemoveManagedTokensAsync()
        {
            var cacheKey = GetCacheKey();
            Session.Remove(cacheKey);
            ManagedTokens = null;
        }
    }
}
