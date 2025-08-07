using GameNewsBoard.Application.DTOs.Responses.Steam;

namespace GameNewsBoard.Application.IServices.ISteam
{
    public interface ISteamApiService
    {
        Task<SteamUserProfileResponse?> GetSteamUserProfileAsync(string steamId);
        Task<List<OwnedGameResponse>> GetOwnedGamesAsync(string steamId);
        Task<List<SteamAchievementResponse>> GetPlayerAchievementsAsync(string steamId, int appId);
        Task<Dictionary<string, double>> GetGlobalAchievementPercentagesAsync(int appId);
    }
}