using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsBoard.Infrastructure.Repositories
{
    public class UploadedImageRepository : GenericRepository<UploadedImage>, IUploadedImageRepository
    {
        public UploadedImageRepository(AppDbContext context) : base(context) { }

        public async Task<List<UploadedImage>> GetUnusedImagesByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(img => img.UserId == userId && !img.IsUsed)
                .ToListAsync();
        }
    }
}