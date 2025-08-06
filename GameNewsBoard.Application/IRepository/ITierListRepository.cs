using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.IRepository
{
    public interface ITierListRepository : IGenericRepository<TierList>
    {
        Task AddEntryAsync(TierListEntry entry);
        Task<List<TierList>> GetByUserIdAsync(Guid userId);
        Task<TierList?> GetDetailedByIdAsync(Guid id);
        Task<(TierList tierList, Guid? imageId)?> GetTierWithImageIdAsync(Guid tierListId);
    }
}