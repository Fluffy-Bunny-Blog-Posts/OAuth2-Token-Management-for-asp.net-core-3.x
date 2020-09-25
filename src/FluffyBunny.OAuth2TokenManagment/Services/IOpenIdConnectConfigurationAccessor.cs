using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment.Services
{
    public interface IOpenIdConnectConfigurationAccessor
    {
        Task<OpenIdConnectConfiguration> GetOpenIdConnectConfigurationAsync(string issuer);
    }
}
