using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.IRepository
{
    public interface IUploadedImageRepository : IGenericRepository<UploadedImage>
    {
        Task<List<UploadedImage>> GetUnusedImagesByUserIdAsync(Guid userId);
    }
}