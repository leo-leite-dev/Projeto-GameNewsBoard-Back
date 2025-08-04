using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Infrastructure.ExternalDtos.Commons;
using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.Igdb.ExternalDtos
{
    public class IgdbGameReleaseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
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