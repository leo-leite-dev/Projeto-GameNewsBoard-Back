using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.DTOs.Requests;

namespace GameNewsBoard.Application.IServices.IGame
{
    public interface IGameStatusService
    {
        Task<Result> SetGameStatusAsync(Guid userId, int gameId, GameStatusRequest request);
        Task<Result<IEnumerable<GameStatusResponse>>> GetUserGameStatusesAsync(Guid userId);
        Task<Result> RemoveGameStatusAsync(Guid userId, int gameId);
    }
}