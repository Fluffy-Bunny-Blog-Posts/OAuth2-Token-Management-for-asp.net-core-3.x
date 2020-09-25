using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Services;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment
{
    public class GlobalDistributedCacheTokenStorage : DistributedCacheTokenStorage<GlobalDistributedCacheTokenStorage>
    {
        const string _cacheKey = "jmu35eC/BBbtV7QODlFVD4QZLR2vUP73Z3vI/1BAQug=.CsBu1d6kaefFmIitghoTyw==";

        public GlobalDistributedCacheTokenStorage(ISerializer serializer, IDataProtectorAccessor dataProtectorAccessor, IDistributedCache cache, ILogger<GlobalDistributedCacheTokenStorage> logger) : base(serializer, dataProtectorAccessor, cache, logger)
        {
        }

        public override Task<string> GetCacheKeyAsync()
        {
            return Task.FromResult(_cacheKey);
        }
    }
}
