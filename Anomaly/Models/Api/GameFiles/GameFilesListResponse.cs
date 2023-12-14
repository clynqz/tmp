using System.Text.Json.Serialization;

namespace Anomaly.Models.Api.GameFiles
{
    public class GameFilesListResponse
    {
        [JsonPropertyName("game_files")]
        public required Dictionary<string, string> GameFiles { get; set; }
    }
}
