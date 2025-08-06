using AutoMapper;
using GameNewsBoard.Application.DTOs.Responses;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Domain.IStorage;

namespace GameNewsBoard.Infrastructure.Services;

public class UploadedImageService : IUploadedImageService
{
    private readonly IUploadedImageRepository _uploadedImageRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;

    public UploadedImageService(
        IUploadedImageRepository uploadedImageRepository,
        IImageStorageService imageStorageService,
        IMapper mapper)
    {
        _uploadedImageRepository = uploadedImageRepository ?? throw new ArgumentNullException(nameof(uploadedImageRepository));
        _imageStorageService = imageStorageService ?? throw new ArgumentNullException(nameof(imageStorageService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<UploadedImageResponse>> RegisterImageAsync(Guid userId, Stream fileStream, string originalFileName, string contentType, ImageBucketCategory category)
    {
        var fileExtension = Path.GetExtension(originalFileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";

        var imageUrl = await _imageStorageService.UploadImageAsync(fileStream, fileName, contentType, userId, category);

        var image = new UploadedImage
        {
            UserId = userId,
            Url = imageUrl
        };

        await _uploadedImageRepository.AddAsync(image);
        await _uploadedImageRepository.SaveChangesAsync();

        var response = _mapper.Map<UploadedImageResponse>(image);
        return Result<UploadedImageResponse>.Success(response);
    }

    public async Task MarkImageAsUsedAsync(Guid imageId)
    {
        var image = await _uploadedImageRepository.GetByIdAsync(imageId);
        if (image != null)
        {
            image.IsUsed = true;
            await _uploadedImageRepository.SaveChangesAsync();
        }
    }

    public async Task<Result> DeleteImageAsync(Guid userId, Guid imageId)
    {
        var image = await _uploadedImageRepository.GetByIdAsync(imageId);

        if (image == null)
            return Result.Failure("Imagem não encontrada.");

        if (image.UserId != userId)
            return Result.Failure("Você não tem permissão para deletar esta imagem.");

        if (image.IsUsed)
            return Result.Failure("Imagem já foi utilizada e não pode ser removida.");

        var filePath = image.Url.Replace($"{_imageStorageService.BasePublicUrl}/", "");
        await _imageStorageService.DeleteImageAsync(filePath);

        await _uploadedImageRepository.DeleteAsync(image);
        await _uploadedImageRepository.SaveChangesAsync();

        return Result.Success();
    }
}