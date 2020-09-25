using FluffyBunny.OAuth2TokenManagment.Services;
using Microsoft.AspNetCore.DataProtection;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    internal class DataProtectorAccessor : IDataProtectorAccessor
    {
        private IDataProtectionProvider _provider;

        public DataProtectorAccessor(IDataProtectionProvider provider)
        {
            _provider = provider;
        }
        public IDataProtector GetAppProtector()
        {
            return GetProtector("5c448492-3b24-4f89-a694-751ca5a1b300");
        }

        public IDataProtector GetProtector(string name)
        {
            var protector = _provider.CreateProtector(name);
            return protector;
        }
    }
}

