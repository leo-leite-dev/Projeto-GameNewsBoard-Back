namespace GameNewsBoard.Application.DTOs.Responses.Steam
{
    public class OwnedGameResponse
    {
        public int AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int PlaytimeForever { get; set; }
        public int AchievementsUnlocked { get; set; }
        public int TotalAchievements { get; set; }
    }
}