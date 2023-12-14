using System.Text.Json.Serialization;

namespace Anomaly.Models.Auth
{
    public class AuthRequest
    {
        [JsonPropertyName("login")]
        public required string Login { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
