using System.Text.Json.Serialization;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos.Base;

namespace GameNewsBoard.Infrastructure.Igdb.ExternalDtos
{
    public class IgdbGameDto : IgdbGameBaseDto
    {
        [JsonPropertyName("aggregated_rating")]
        public float? AggregatedRating { get; set; }

        [JsonPropertyName("rating")]
        public float? UserRating { get; set; }

        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDateUnix { get; set; }
    }
}