using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos.Base;
using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.Igdb.ExternalDtos
{
    public class IgdbGameReleaseDto : IgdbGameBaseDto
    {
        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }

        [JsonPropertyName("category")]
        public GameCategory? Category { get; set; }

        [JsonPropertyName("version_parent")]
        public long? VersionParent { get; set; }

    }
}