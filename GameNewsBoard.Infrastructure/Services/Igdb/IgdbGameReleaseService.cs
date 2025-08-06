using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.IServices.IGame;
using GameNewsBoard.Application.IServices.Igdb;
using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Domain.Platforms;
using GameNewsBoard.Infrastructure.Configurations.Settings;
using GameNewsBoard.Infrastructure.Helpers;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos;
using GameNewsBoard.Infrastructure.Service.IGDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsBoard.Infrastructure.Services.Igdb
{
    public class IgdbGameReleaseService : IgdbApiBaseService, IIgdbGameReleaseService
    {
        private readonly IIgdbQueryBuilder _queryBuilder;
        private readonly IMapper _mapper;
        private readonly ILogger<IgdbGameReleaseService> _logger;

        public IgdbGameReleaseService(HttpClient httpClient,
                                  IOptions<IgdbSettings> igdbOptions,
                                  IIgdbQueryBuilder queryBuilder,
                                  IMapper mapper,
                                  ILogger<IgdbGameReleaseService> logger)
            : base(httpClient, igdbOptions.Value.ClientId, igdbOptions.Value.AccessToken)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<GameReleaseResponse>> GetTodayReleasesGamesAsync(
            PlatformFamily? platform = null, CancellationToken cancellationToken = default)
        {
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(1).AddSeconds(-1);
            return await GetReleasesBetweenAsync(start, end, platform, cancellationToken);
        }

        public async Task<List<GameReleaseResponse>> GetUpcomingGamesAsync(
            int daysAhead = 7, PlatformFamily? platform = null, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var future = now.AddDays(daysAhead);
            return await GetReleasesBetweenAsync(now, future, platform, cancellationToken);
        }

        public async Task<List<GameReleaseResponse>> GetRecentlyReleasedGamesAsync(
            int daysBack = 7, PlatformFamily? platform = null, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow.Date.AddSeconds(-1);
            var past = DateTime.UtcNow.Date.AddDays(-daysBack);
            return await GetReleasesBetweenAsync(past, now, platform, cancellationToken);
        }

        public async Task<List<GameReleaseResponse>> GetReleasesBetweenAsync(
            DateTime start, DateTime end, PlatformFamily? platform = null, CancellationToken cancellationToken = default)
        {
            return await LoggingExecutionHelper.ExecuteWithLoggingAsync(
                async () =>
                {
                    var startUnix = new DateTimeOffset(start).ToUnixTimeSeconds();
                    var endUnix = new DateTimeOffset(end).ToUnixTimeSeconds();

                    var query = _queryBuilder.GenerateGamesReleasedBetweenQuery(startUnix, endUnix);
                    var request = CreateIgdbRequest(query, "games");

                    var igdbGames = await SendIgdbRequestAsync<List<IgdbGameReleaseDto>>(request, cancellationToken);

                    var allowedCategories = new[]
                    {
                GameCategory.MainGame,
                GameCategory.Remake,
                GameCategory.Remaster
                    };

                    var validPlatformIds = Enum.GetValues(typeof(Platform))
                        .Cast<Platform>()
                        .Where(p => p != Platform.All)
                        .Select(p => (int)p)
                        .ToHashSet();

                    var filteredGames = igdbGames
                        .Where(g =>
                            g.Category.HasValue &&
                            allowedCategories.Contains(g.Category.Value) &&
                            g.Platforms is { Count: > 0 } &&
                            (
                                g.Platforms.Any(p => validPlatformIds.Contains(p.Id)) ||
                                g.Platforms.Any(p =>
                                    PlatformLineage.XboxFamily.Values.SelectMany(x => x).Contains(p.Name) ||
                                    PlatformLineage.PlaystationFamily.Values.SelectMany(x => x).Contains(p.Name) ||
                                    PlatformLineage.NintendoFamily.Values.SelectMany(x => x).Contains(p.Name) ||
                                    PlatformLineage.MicrosoftFamily.Values.SelectMany(x => x).Contains(p.Name)
                                )
                            ) &&
                            (
                                platform == null || platform == PlatformFamily.All
                                    ? true
                                    : g.Platforms.Any(p =>
                                        PlatformLineage.XboxFamily.TryGetValue(platform.Value, out var xbox) && xbox.Contains(p.Name) ||
                                        PlatformLineage.PlaystationFamily.TryGetValue(platform.Value, out var ps) && ps.Contains(p.Name) ||
                                        PlatformLineage.NintendoFamily.TryGetValue(platform.Value, out var nintendo) && nintendo.Contains(p.Name) ||
                                        PlatformLineage.MicrosoftFamily.TryGetValue(platform.Value, out var ms) && ms.Contains(p.Name)
                                    )
                            )
                        )
                        .ToList();

                    var rawReleases = _mapper.Map<List<RawGameReleaseDto>>(filteredGames);
                    return _mapper.Map<List<GameReleaseResponse>>(rawReleases);
                },
                _logger,
                "Erro ao buscar lançamentos de jogos."
            );
        }
    }
}