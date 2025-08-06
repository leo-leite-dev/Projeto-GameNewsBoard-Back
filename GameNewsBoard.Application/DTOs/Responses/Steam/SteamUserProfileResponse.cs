namespace GameNewsBoard.Application.DTOs.Responses.Steam
{
    public class SteamUserProfileResponse
    {
        public string PersonaName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public long? LastLogOff { get; set; }
        public int PersonaState { get; set; }
        public string? PrimaryClanId { get; set; }
        public long? TimeCreated { get; set; }
        public string? LocCountryCode { get; set; }
        public string? GameId { get; set; }
        public string? GameExtraInfo { get; set; }
    }
}