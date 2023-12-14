using System.Text.Json.Serialization;

namespace Anomaly.Models.Auth
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }
    }
}
