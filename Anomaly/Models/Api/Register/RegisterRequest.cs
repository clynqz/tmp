using System.Text.Json.Serialization;

namespace Anomaly.Models.Register
{
    public class RegisterRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("nickname")]
        public required string Nickname { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
