using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Infrastructure.ExternalDtos.Commons;
using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.ExternalDtos
{
    public class IgdbGameReleaseDto
    {
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }

        public IgdbCoverDto? Cover { get; set; }

        public List<IgdbPlatformDto>? Platforms { get; set; }
        [JsonPropertyName("category")]
        public GameCategory? Category { get; set; }

        [JsonPropertyName("version_parent")]
        public long? VersionParent { get; set; }

    }
}