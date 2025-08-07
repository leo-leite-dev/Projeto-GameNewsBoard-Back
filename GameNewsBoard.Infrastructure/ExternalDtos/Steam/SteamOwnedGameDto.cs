using System.Text.Json.Serialization;

namespace GameNewsBoard.Infrastructure.ExternalDtos.Steam
{
    public class SteamOwnedGamesResponseDto
    {
        [JsonPropertyName("response")]
        public SteamOwnedGamesInnerDto Response { get; set; } = new();
    }

    public class SteamOwnedGamesInnerDto
    {
        [JsonPropertyName("game_count")]
        public int GameCount { get; set; }

        [JsonPropertyName("games")]
        public List<SteamOwnedGameDto> Games { get; set; } = new();
    }

    public class SteamOwnedGameDto
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonPropertyName("img_logo_url")]
        public string ImgLogoUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("rtime_last_played")]
        public long? LastPlayedUnix { get; set; }
    }
}