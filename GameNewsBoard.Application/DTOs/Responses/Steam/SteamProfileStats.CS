namespace GameNewsBoard.Application.DTOs.Responses.Steam
{
    public record SteamProfileStats(
        int TotalAchievements,
        int CompletedGamesCount,
        double TotalCompletionPercentage,
        SteamAchievementResponse? LastUnlockedAchievement,
        SteamAchievementResponse? RarestAchievement
    );
}