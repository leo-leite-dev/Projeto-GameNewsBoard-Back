using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Domain.Commons;

namespace GameNewsBoard.Application.IServices.ISteam
{
    public interface ISteamUserService
    {
        Task<SteamUserProfileResponse?> GetCompleteSteamUserProfileAsync(string steamId);
        Task<SteamUserProfileResponse?> GetCompleteSteamUserProfileAsync(Guid userId);
        Task<Result> LinkSteamAccountAsync(Guid userId, string steamId);
    }
}