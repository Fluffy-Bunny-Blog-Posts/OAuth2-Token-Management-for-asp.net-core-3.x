using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IFakeTokenFetchService
    {
        Task<TokenResponse> GetTokenExchangeAsync(string idToken);
    }
}
