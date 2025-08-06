using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Application.DTOs.Responses.TierList;

namespace GameNewsBoard.Application.DTOs.Responses.User
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public List<TierListResponse> Tiers { get; set; } = new();
        public List<GameStatusResponse> GameStatuses  { get; set; } = new();

        public SteamUserProfileResponse? SteamProfile { get; set; }
    }
}