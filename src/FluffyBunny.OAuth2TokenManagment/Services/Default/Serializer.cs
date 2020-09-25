using FluffyBunny.OAuth2TokenManagment.Services;
using System.Text.Json;

namespace FluffyBunny.OAuth2TokenManagment.Services.Default
{
    internal class Serializer : ISerializer
    {
        public T Deserialize<T>(string text) where T : class
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(text, options);
        }

        public string Serialize<T>(T obj) where T : class
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = false,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
