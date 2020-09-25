using IdentityModel.Client;

namespace FluffyBunny.OAuth2TokenManagment.Models
{
    public class OAuth2Credentials
    {
        public string Authority { get; set; }
        public DiscoveryDocumentResponse DiscoveryDocumentResponse { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

    }
}
