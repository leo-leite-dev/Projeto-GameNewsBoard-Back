using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.ExternalDtos.Steam
{
    public class SteamPlayerAchievementsResponseDto
    {
        [JsonPropertyName("playerstats")]
        public SteamPlayerAchievementsStatsDto PlayerStats { get; set; } = new();
    }

    public class SteamPlayerAchievementsStatsDto
    {
        [JsonPropertyName("steamID")]
        public string SteamId { get; set; } = string.Empty;

        [JsonPropertyName("gameName")]
        public string GameName { get; set; } = string.Empty;

        [JsonPropertyName("achievements")]
        public List<SteamAchievementDto> Achievements { get; set; } = new();
    }

    public class SteamAchievementDto
    {
        [JsonPropertyName("apiname")]
        public string ApiName { get; set; } = string.Empty;

        [JsonPropertyName("achieved")]
        public int Achieved { get; set; }

        [JsonPropertyName("unlocktime")]
        public long UnlockTimeUnix { get; set; }
    }
}
