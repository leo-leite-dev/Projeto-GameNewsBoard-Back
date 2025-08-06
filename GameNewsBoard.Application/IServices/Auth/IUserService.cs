using GameNewsBoard.Application.DTOs.Requests;
using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.IServices.Auth
{
    public interface IAuthService
    {
        Task<Result<User>> AuthenticateAsync(LoginRequest request);
        Task<Result> RegisterAsync(RegisterRequest request);
    }
}