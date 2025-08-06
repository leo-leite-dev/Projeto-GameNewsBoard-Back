using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsBoard.Infrastructure.Repositories
{
    public class TierListRepository : GenericRepository<TierList>, ITierListRepository
    {
        public TierListRepository(AppDbContext context) : base(context) { }


        public async Task AddEntryAsync(TierListEntry entry)
        {
            await _context.Set<TierListEntry>().AddAsync(entry);
        }

        public async Task<List<TierList>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(t => t.Entries)
                .ThenInclude(e => e.Game)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<TierList?> GetDetailedByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Entries)
                .ThenInclude(e => e.Game)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(TierList tierList, Guid? imageId)?> GetTierWithImageIdAsync(Guid tierListId)
        {
            var tierList = await _dbSet
                .FirstOrDefaultAsync(t => t.Id == tierListId);

            if (tierList == null)
                return null;

            return (tierList, tierList.ImageId);
        }
    }
}