using GameNewsBoard.Domain.IStorage;

namespace GameNewsBoard.Application.Services
{
    public class ImageService
    {
        private readonly IImageStorageService _storage;

        public ImageService(IImageStorageService storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public Task<string> UploadAsync(Stream stream, string fileName, string contentType)
        {
            return _storage.UploadImageAsync(stream, fileName, contentType);
        }
    }
}