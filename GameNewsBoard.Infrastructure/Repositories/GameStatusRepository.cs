using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsBoard.Infrastructure.Repositories
{
    public class GameStatusRepository : GenericRepository<GameStatus>, IGameStatusRepository
    {
        public GameStatusRepository(AppDbContext context) : base(context) { }

        public async Task<List<GameStatus>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(gs => gs.Game)
                .Where(gs => gs.UserId == userId)
                .ToListAsync();
        }

        public async Task<GameStatus?> GetByUserAndGameIdAsync(Guid userId, int gameId)
        {
            return await _dbSet
                .Include(gs => gs.Game)
                .FirstOrDefaultAsync(gs => gs.UserId == userId && gs.GameId == gameId);
        }
    }
}