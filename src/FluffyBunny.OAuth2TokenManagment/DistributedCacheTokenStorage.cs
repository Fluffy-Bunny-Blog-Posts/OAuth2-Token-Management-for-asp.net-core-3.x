using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace FluffyBunny.OAuth2TokenManagment
{
    public abstract class DistributedCacheTokenStorage<T>: TokenStorage
        where T: TokenStorage
    {
        protected ISerializer _serializer;
        protected IDataProtectorAccessor _dataProtectorAccessor;
        private IDistributedCache _cache;
        protected ILogger<T> _logger;
        public DistributedCacheTokenStorage(
            ISerializer serializer,
            IDataProtectorAccessor dataProtectorAccessor,
            IDistributedCache cache, 
            ILogger<T> logger)
        {
            _serializer = serializer;
            _dataProtectorAccessor = dataProtectorAccessor;
            _cache = cache;
            _logger = logger;
        }
        public abstract Task<string> GetCacheKeyAsync();

        protected async Task<Dictionary<string, ManagedToken>> GetManagedTokensAsync()
        {
            var cacheKey = await GetCacheKeyAsync();
            var protector = _dataProtectorAccessor.GetProtector(cacheKey);
           
            var json = await _cache.GetObjectFromJson<string>(cacheKey);
            
            Dictionary<string, ManagedToken> managedTokens = null;
            if (string.IsNullOrWhiteSpace(json))
            {
                managedTokens = new Dictionary<string, ManagedToken>();
                await UpsertManagedTokensAsync(managedTokens);
                return managedTokens;
            }
            json = protector.Unprotect(json);
           
            var tokens = _serializer.Deserialize<Dictionary<string, ManagedToken>>(json);
            return tokens;
        }
       
        protected async Task UpsertManagedTokensAsync(Dictionary<string, ManagedToken> managedTokens)
        {
            var cacheKey = await GetCacheKeyAsync();
            var protector = _dataProtectorAccessor.GetProtector(cacheKey);
          
            var json = _serializer.Serialize(managedTokens);
            json = protector.Protect(json);
            await _cache.SetObjectAsJson(cacheKey, json, new DistributedCacheEntryOptions
            {
                SlidingExpiration = new TimeSpan(1, 0, 0, 0)// 1 day sliding
            });
        }
        public async override Task<ManagedToken> GetManagedTokenAsync(string key)
        {
            var tokens = await GetManagedTokensAsync();
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
            var tokens = await GetManagedTokensAsync();
            if (tokens == null)
            {
                return;
            }
            if (tokens.ContainsKey(key))
            {
                tokens.Remove(key);
                await UpsertManagedTokensAsync(tokens);
            }
        }

        public async override Task UpsertManagedTokenAsync(string key, ManagedToken managedToken)
        {
            var tokens = await GetManagedTokensAsync();
            if (tokens == null)
            {
                tokens = new Dictionary<string, ManagedToken>();
            }
            tokens[key] = managedToken;
            await UpsertManagedTokensAsync(tokens);
        }

        public async override Task RemoveManagedTokensAsync()
        {
            var cacheKey = await GetCacheKeyAsync();
            await _cache.RemoveAsync(cacheKey);
        }
        
    }
}
