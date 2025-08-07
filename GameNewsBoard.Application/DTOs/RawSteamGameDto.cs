namespace GameNewsBoard.Application.DTOs
{
    public class RawSteamGameDto
    {
        public int AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public int PlaytimeForever { get; set; }
        public DateTimeOffset? LastPlayed { get; set; }
    }
}