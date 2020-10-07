using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface IDataProtectorAccessor
    {
        IDataProtector GetAppProtector();
        IDataProtector GetProtector(string name);
    }
}

