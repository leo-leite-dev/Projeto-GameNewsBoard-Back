using AutoMapper;
using GameNewsBoard.Application.Commons;
using GameNewsBoard.Application.DTOs.Requests;
using GameNewsBoard.Application.DTOs.Responses.TierList;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Domain.IStorage;

namespace GameNewsBoard.Infrastructure.Services
{
    public class TierListService : ITierListService
    {
        private readonly ITierListRepository _tierListRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IUploadedImageRepository _uploadedImageRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IMapper _mapper;

        public TierListService(
            ITierListRepository tierListRepository,
            IGameRepository gameRepository,
            IUploadedImageRepository uploadedImageRepository,
            IImageStorageService imageStorageService,
            IMapper mapper)
        {
            _tierListRepository = tierListRepository ?? throw new ArgumentNullException(nameof(tierListRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _uploadedImageRepository = uploadedImageRepository ?? throw new ArgumentNullException(nameof(uploadedImageRepository));
            _imageStorageService = imageStorageService ?? throw new ArgumentNullException(nameof(imageStorageService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> CreateTierListAsync(Guid userId, TierListRequest request)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var tierList = TierList.Create(userId, request.Title, request.ImageId, request.ImageUrl);

                await _tierListRepository.AddAsync(tierList);
                await _tierListRepository.SaveChangesAsync();

                if (request.ImageId.HasValue)
                {
                    var image = await _uploadedImageRepository.GetByIdAsync(request.ImageId.Value);

                    if (image != null && !image.IsUsed)
                    {
                        image.ImageInUsed();
                        await _uploadedImageRepository.SaveChangesAsync();
                    }
                }
            }, "Ocorreu um erro ao criar a lista de tiers. Tente novamente mais tarde.");
        }

        public async Task<Result> UpdateTierListAsync(Guid tierListId, UpdateTierListRequest request)
        {
            var tierList = await _tierListRepository.GetByIdAsync(tierListId);
            if (tierList == null)
                return Result.Failure("Tier não encontrado.");

            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                tierList.UpdateInfo(request.Title, request.ImageUrl);
                await _tierListRepository.SaveChangesAsync();
            }, "Erro ao atualizar a tier list.");
        }

        public async Task<Result> DeleteTierListAsync(Guid tierListId)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var tierListData = await _tierListRepository.GetTierWithImageIdAsync(tierListId);
                if (tierListData == null)
                    throw new ArgumentException("Tier não encontrado.");

                var (tierList, imageId) = tierListData.Value;

                if (imageId.HasValue)
                {
                    var image = await _uploadedImageRepository.GetByIdAsync(imageId.Value);
                    if (image != null)
                    {
                        await _uploadedImageRepository.DeleteAsync(image);
                        await _imageStorageService.DeleteImageAsync(image.Url);
                    }
                }

                await _tierListRepository.DeleteAsync(tierList);
                await _tierListRepository.SaveChangesAsync();
            }, "Ocorreu um erro ao excluir a tier list. Tente novamente.");
        }

        public async Task<Result<IEnumerable<TierListResponse>>> GetTierListsByUserAsync(Guid userId)
        {
            var tiers = await _tierListRepository.GetByUserIdAsync(userId);
            var response = _mapper.Map<IEnumerable<TierListResponse>>(tiers);
            return Result<IEnumerable<TierListResponse>>.Success(response);
        }

        public async Task<Result> SetGameTierAsync(Guid tierListId, TierListEntryRequest request)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var tierList = await _tierListRepository.GetByIdAsync(tierListId)
                               ?? throw new ArgumentException("Tier não encontrado.");

                var game = await _gameRepository.GetByIdAsync(request.GameId)
                            ?? throw new ArgumentException("Jogo não encontrado.");

                var existingEntry = tierList.Entries.FirstOrDefault(e => e.GameId == request.GameId);

                if (existingEntry is null)
                {
                    var newEntry = TierListEntry.Create(request.GameId, request.Tier, tierList.Id);
                    await _tierListRepository.AddEntryAsync(newEntry);
                }
                else
                {
                    existingEntry.UpdateTier(request.Tier);
                }

                await _tierListRepository.SaveChangesAsync();

            }, "Erro ao definir o tier do jogo. Tente novamente.");
        }

        public async Task<Result> RemoveGameFromTierAsync(Guid tierListId, int gameId)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var tierList = await _tierListRepository.GetByIdAsync(tierListId)
                               ?? throw new ArgumentException("Tier não encontrado.");

                tierList.RemoveGameFromTier(gameId);
                await _tierListRepository.SaveChangesAsync();
            }, "Erro ao remover jogo do tier. Tente novamente.");
        }

        public async Task<Result<TierListResponse>> GetTierListByIdAsync(Guid tierListId)
        {
            var tierList = await _tierListRepository.GetByIdAsync(tierListId);

            if (tierList == null)
                return Result<TierListResponse>.Failure("Tier não encontrado.");

            var response = _mapper.Map<TierListResponse>(tierList);
            return Result<TierListResponse>.Success(response);
        }
    }
}