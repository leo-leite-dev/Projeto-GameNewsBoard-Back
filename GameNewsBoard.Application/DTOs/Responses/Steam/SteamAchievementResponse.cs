namespace GameNewsBoard.Application.DTOs.Responses.Steam
{
    public class SteamAchievementResponse
    {
        public string Name { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; }
        public DateTimeOffset? UnlockedAt { get; set; }
    }
}