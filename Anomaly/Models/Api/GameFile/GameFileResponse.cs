using System.Text.Json.Serialization;

namespace Anomaly.Models.Api.GameFile
{
    public class GameFileResponse
    {
        [JsonPropertyName("content")]
        public required byte[] Content { get; set; }
    }
}
