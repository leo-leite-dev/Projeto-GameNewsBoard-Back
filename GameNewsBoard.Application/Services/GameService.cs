using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.DTOs.Shared;
using GameNewsBoard.Application.Exceptions;
using GameNewsBoard.Application.Exceptions.Domain;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.IGame;
using GameNewsBoard.Application.IServices.Igdb;
using GameNewsBoard.Application.Validators;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.Services
{

    public class GameService : IGameService
    {
        private readonly IIgdbQueryBuilder _igdbQueryBuilder;
        private readonly IIgdbApiService _igdbApiService;
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;


        public GameService(
            IIgdbQueryBuilder igdbQueryBuilder,
            IIgdbApiService igdbApiService,
            IGameRepository gameRepository,
            IMapper mapper
            )
        {
            _igdbQueryBuilder = igdbQueryBuilder ?? throw new ArgumentNullException(nameof(igdbQueryBuilder));
            _igdbApiService = igdbApiService ?? throw new ArgumentNullException(nameof(igdbApiService));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginatedFromApiResult<GameResponse>> GetPaginedGamesAsync(
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            PaginationValidator.Validate(page, pageSize);

            var query = _igdbQueryBuilder.GenerateGamesQuery(page, pageSize);

            var rawGames = await _igdbApiService.GetRawGamesAsync(query, cancellationToken);
            if (rawGames.Count == 0)
                return PaginatedFromApiResult<GameResponse>.Empty(page, pageSize);

            var normalizedTitles = rawGames.Select(r => r.Title.Trim().ToLowerInvariant()).ToList();
            var savedGamesDict = (await _gameRepository.GetByTitlesAsync(normalizedTitles))
                .ToDictionary(g => g.Title.Trim().ToLowerInvariant(), g => g.Id);

            var responses = rawGames.Select(raw =>
            {
                var response = _mapper.Map<GameResponse>(raw);
                response.Id = savedGamesDict.GetValueOrDefault(raw.Title.Trim().ToLowerInvariant(), 0);
                return response;
            }).ToList();

            return new PaginatedFromApiResult<GameResponse>(responses, page, pageSize);
        }

        public async Task SaveGamesAsync(int batchSize = 500, CancellationToken cancellationToken = default)
        {
            int offset = 0;
            bool hasMoreGames = true;

            while (hasMoreGames)
            {
                var query = _igdbQueryBuilder.GenerateGamesQueryWithOffset(offset, batchSize);
                var rawGames = await _igdbApiService.GetRawGamesAsync(query, cancellationToken);

                if (rawGames == null || !rawGames.Any())
                {
                    hasMoreGames = false;
                    continue;
                }

                var distinctRawGames = rawGames
                    .Where(g => !string.IsNullOrWhiteSpace(g.Title))
                    .GroupBy(g => g.Title.Trim().ToLowerInvariant())
                    .Select(g => g.First())
                    .ToList();

                var normalizedTitles = distinctRawGames
                    .Select(g => g.Title.Trim().ToLowerInvariant())
                    .ToList();

                var existingTitles = (await _gameRepository.GetByNormalizedTitlesAsync(normalizedTitles))
                    .Select(g => g.Title.Trim().ToLowerInvariant())
                    .ToHashSet();

                var newGamesToSave = distinctRawGames
                    .Where(g => !existingTitles.Contains(g.Title.Trim().ToLowerInvariant()))
                    .Select(raw =>
                    {
                        var game = _mapper.Map<Game>(raw);
                        game.Released = game.Released.ToUniversalTime();
                        return game;
                    })
                    .ToList();

                if (newGamesToSave.Count > 0)
                    await _gameRepository.AddGamesAsync(newGamesToSave);

                offset += batchSize;
            }
        }

        public async Task<PaginatedResult<GameDTO>> GetGameExclusiveByPlatformAsync(
            Platform? platform,
            string? searchTerm,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                PlatformSearchValidator.Validate(platform, searchTerm);
                PaginationValidator.Validate(page, pageSize);

                var offset = (page - 1) * pageSize;
                var platformId = platform.HasValue ? (int)platform.Value : 0;
                var platformName = platform.HasValue
                    ? PlatformMapping.GetPlatformName(platform.Value)
                    : "Unknown Platform";

                if (platformName == "Unknown Platform" && platformId != 0)
                    throw new InvalidPlatformException($"Plataforma com ID {platformId} não encontrada.");

                var result = await _gameRepository.GetGamesExclusivePlatformAsync(
                    platformId, searchTerm, offset, pageSize, cancellationToken);

                var gameDTOs = _mapper.Map<IEnumerable<GameDTO>>(result.games);
                var totalPages = (int)Math.Ceiling((double)result.totalCount / pageSize);

                return new PaginatedResult<GameDTO>(
                    gameDTOs.ToList(),
                    page,
                    pageSize,
                    result.totalCount,
                    totalPages);
            }
            catch (InvalidPlatformException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedResult<GameDTO>> GetGamesByYearCategoryAsync(
            YearCategory? yearCategory,
            string? searchTerm,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (yearCategory is null)
                    throw new InvalidYearCategoryException("A categoria de ano não pode ser nula.");

                YearCategoryValidator.Validate(yearCategory);
                PaginationValidator.Validate(page, pageSize);

                var category = yearCategory.Value;
                var offset = (page - 1) * pageSize;

                int? startYear = null, endYear = null;

                switch (category)
                {
                    case YearCategory.Classics:
                        startYear = 1980;
                        endYear = 1999;
                        break;

                    case YearCategory.Recent:
                        startYear = 2000;
                        endYear = 2024;
                        break;

                    case YearCategory.Release:
                        startYear = 2025;
                        endYear = 2025;
                        break;

                    case YearCategory.All:
                        break;
                }

                var result = await _gameRepository.GetGamesByYearCategoryAsync(
                    startYear, endYear, searchTerm, offset, pageSize, cancellationToken);

                var gameDTOs = _mapper.Map<IEnumerable<GameDTO>>(result.games);
                var totalPages = (int)Math.Ceiling((double)result.totalCount / pageSize);

                return new PaginatedResult<GameDTO>(
                    gameDTOs.ToList(),
                    page,
                    pageSize,
                    result.totalCount,
                    totalPages);
            }
            catch (InvalidYearCategoryException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}