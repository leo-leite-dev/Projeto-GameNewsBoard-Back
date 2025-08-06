using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.External.Steam.Dtos
{
    public class SteamPlayerResponseDto
    {
        [JsonPropertyName("response")]
        public SteamPlayerResponseInner Response { get; set; } = new();
    }

    public class SteamPlayerResponseInner
    {
        [JsonPropertyName("players")]
        public List<SteamPlayerDto> Players { get; set; } = new();
    }

    public class SteamPlayerDto
    {
        [JsonPropertyName("steamid")]
        public string SteamId { get; set; } = string.Empty;

        [JsonPropertyName("personaname")]
        public string Personaname { get; set; } = string.Empty;

        [JsonPropertyName("avatarfull")]
        public string Avatarfull { get; set; } = string.Empty;

        [JsonPropertyName("lastlogoff")]
        public long? LastLogOff { get; set; }

        [JsonPropertyName("personastate")]
        public int PersonaState { get; set; }

        [JsonPropertyName("primaryclanid")]
        public string? PrimaryClanId { get; set; }

        [JsonPropertyName("timecreated")]
        public long? TimeCreated { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string? LocCountryCode { get; set; }

        [JsonPropertyName("gameid")]
        public string? GameId { get; set; }

        [JsonPropertyName("gameextrainfo")]
        public string? GameExtraInfo { get; set; }
    }
}