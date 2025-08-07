namespace GameNewsBoard.Infrastructure.ExternalDtos.Steam
{
    public class SteamGlobalAchievementsResponseDto
    {
        public GlobalAchievementResponse Response { get; set; } = new();
    }

    public class GlobalAchievementResponse
    {
        public List<GlobalAchievementDto> Achievementpercentages { get; set; } = new();
    }

    public class GlobalAchievementDto
    {
        public string Name { get; set; } = string.Empty;
        public double Percent { get; set; }
    }
}