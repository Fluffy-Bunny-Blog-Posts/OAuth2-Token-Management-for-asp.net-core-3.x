using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace Microsoft.Extensions.Caching.Distributed
{
    internal static class DistributedCacheExtensions
    {
        public static async Task SetObjectAsJson<T>(
               this IDistributedCache cache, string key, T item,
               DistributedCacheEntryOptions cacheOptions)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = false,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(item, options);

            await cache.SetStringAsync(key, json, cacheOptions);
        }
        public static async Task SetObjectAsJson<T>(
            this IDistributedCache cache, string key, T item,
            int expirationInMinutes)
        {
            await SetObjectAsJson<T>(cache,key, item, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
            });
        }
        public static async Task<T> GetObjectFromJson<T>(this IDistributedCache cache, string key)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true
            };
            var json = await cache.GetStringAsync(key);
           
            return json == null ? default(T) :
                JsonSerializer.Deserialize<T>(json, options);
        }

    }
}
