namespace GameNewsBoard.Application.DTOs.Responses.Steam
{
    public class SteamUserProfileResponse
    {
        public string SteamId { get; set; } = string.Empty;
        public string PersonaName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public long? LastLogOff { get; set; }
        public int PersonaState { get; set; }

        public List<OwnedGameResponse> Games { get; set; } = new();
        public int TotalAchievements { get; set; }

        public int CompletedGamesCount { get; set; }           // ✅ Jogos 100%
        public double AverageCompletionPercent { get; set; }    // 📊 Média
        public SteamAchievementResponse? LastUnlockedAchievement { get; set; } // 🕓
        public SteamAchievementResponse? RarestAchievement { get; set; }       // 🧊
    }
}