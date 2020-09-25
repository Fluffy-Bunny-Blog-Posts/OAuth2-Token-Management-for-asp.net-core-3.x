using System;
using System.Text.Json.Serialization;

namespace FluffyBunny.OAuth2TokenManagment.Models
{
    /*
     * 
     
    {
      "refresh_token": "",
      "access_token": "",
      "start_date": "0001-01-01T00:00:00+00:00",
      "expires_in": 0,
      "authority": "",
      "client_id": "",
      "client_secret":"",
      "token_endpoint": "https://demo.identityserver.io"
    }


     * 
     */
    public class ManagedToken
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("start_date")]
        public DateTimeOffset StartDate { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("credentials_key")]
        public string CredentialsKey { get; set; }
        [JsonPropertyName("request_function_key")]
        public string RequestFunctionKey { get; set; }

        // This is calculated based upon StartDate + ExpiresIn
        [JsonPropertyName("expiration_date")]
        public DateTimeOffset ExpirationDate { get; set; }

        [JsonPropertyName("requested_scope")]
        public string RequestedScope { get; set; }
    }
}
