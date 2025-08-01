using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Domain.IStorage
{
    public interface IImageStorageService
    {
        string BasePublicUrl { get; }
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, Guid userId, ImageBucketCategory category);
        Task DeleteImageAsync(string filePath);
    }
}