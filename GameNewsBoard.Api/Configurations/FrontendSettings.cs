namespace GameNewsBoard.Api.Configurations
{
    public record FrontendSettings
    {
        public string RedirectAfterSteamLogin { get; init; } = string.Empty;
    }
}