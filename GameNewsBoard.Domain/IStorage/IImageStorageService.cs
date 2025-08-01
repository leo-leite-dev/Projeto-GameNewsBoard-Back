namespace GameNewsBoard.Domain.IStorage
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteImageAsync(string filePath);
    }
}