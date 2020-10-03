using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class FakeTokenFetchService : IFakeTokenFetchService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _httpClient;

        public FakeTokenFetchService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _httpClient = _clientFactory.CreateClient("external");
        }

        public async Task<TokenResponse> GetTokenExchangeAsync(string idToken)
        { 
            var response = await _httpClient.GetAsync($"/api/FakeOAuth2/fake-token-exchange?idToken={idToken}");
            if (response.IsSuccessStatusCode)
            {
                string stringData = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(stringData);

                return tokenResponse;
            }
            return null;
        }
    }
}
