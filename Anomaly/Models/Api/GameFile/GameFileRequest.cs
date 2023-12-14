using System.Text.Json.Serialization;

namespace Anomaly.Models.Api.GameFile
{
    public class GameFileRequest
    {
        [JsonPropertyName("file_path")]
        public required string FilePath { get; set; }
    }
}