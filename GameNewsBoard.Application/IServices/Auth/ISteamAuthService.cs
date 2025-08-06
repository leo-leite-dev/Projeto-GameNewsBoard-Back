using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.IServices.Auth
{
    public interface ISteamAuthService
    {
        Task<User> AuthenticateOrCreateSteamUserAsync(string steamId);
    }
}