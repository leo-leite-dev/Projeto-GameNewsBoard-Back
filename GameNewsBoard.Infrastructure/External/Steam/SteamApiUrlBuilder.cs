namespace GameNewsBoard.Infrastructure.External.Igdb.Steam
{
    public static class SteamApiUrlBuilder
    {
        public static string GetPlayerSummaryUrl(string apiKey, string steamId) =>
            $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={steamId}";

        public static string GetOwnedGamesUrl(string apiKey, string steamId) =>
            $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={apiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";

        public static string GetPlayerAchievementsUrl(string apiKey, string steamId, int appId) =>
            $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={apiKey}&steamid={steamId}&appid={appId}";
    }
}