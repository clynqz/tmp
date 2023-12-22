using System.Text.Json.Serialization;

namespace Anomaly.Models.Auth
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        [JsonPropertyName("nickname")]
        public required string Nickname { get; set; }
    }
}
