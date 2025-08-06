using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.IRepository
{
    public interface IGameStatusRepository : IGenericRepository<GameStatus>
    {
        Task<List<GameStatus>> GetByUserIdAsync(Guid userId);
        Task<GameStatus?> GetByUserAndGameIdAsync(Guid userId, int gameId);
    }
}