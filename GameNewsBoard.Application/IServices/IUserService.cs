using GameNewsBoard.Application.DTOs.Responses.User;

namespace GameNewsBoard.Application.IServices
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetUserProfileAsync(Guid userId);
    }
}