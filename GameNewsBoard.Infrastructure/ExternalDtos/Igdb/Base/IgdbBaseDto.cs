using System.Text.Json.Serialization;
using GameNewsBoard.Infrastructure.ExternalDtos.Commons;

namespace GameNewsBoard.Infrastructure.Igdb.ExternalDtos.Base
{
    public class IgdbGameBaseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cover")]
        public IgdbCoverDto? Cover { get; set; }

        [JsonPropertyName("platforms")]
        public List<IgdbPlatformDto> Platforms { get; set; } = new();
    }
}