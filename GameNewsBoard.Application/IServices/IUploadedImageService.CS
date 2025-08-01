using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.IServices
{
    public interface IUploadedImageService
    {
        Task<Result<UploadedImageDto>> RegisterImageAsync(Guid userId, Stream fileStream, string originalFileName, string contentType, ImageBucketCategory category);
        Task MarkImageAsUsedAsync(Guid imageId);
        Task<Result> DeleteImageAsync(Guid userId, Guid imageId);
    }
}