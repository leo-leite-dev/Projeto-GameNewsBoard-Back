using AutoMapper;
using GameNewsBoard.Application.Exceptions.Api;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Application.Responses.DTOs.Responses;
using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Domain.Platforms;
using GameNewsBoard.Infrastructure.Commons;
using GameNewsBoard.Infrastructure.Configurations;
using GameNewsBoard.Infrastructure.ExternalDtos;
using GameNewsBoard.Infrastructure.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsBoard.Infrastructure.Services;

public class GameReleaseService : IgdbApiBaseService, IGameReleaseService
{
    private readonly IMapper _mapper;
    private readonly ILogger<GameReleaseService> _logger;

    public GameReleaseService(HttpClient httpClient,
                              IOptions<IgdbSettings> igdbOptions,
                              IMapper mapper,
                              ILogger<GameReleaseService> logger)
        : base(httpClient,
               igdbOptions.Value.ClientId,
               igdbOptions.Value.AccessToken)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        var startUnix = new DateTimeOffset(start).ToUnixTimeSeconds();
        var endUnix = new DateTimeOffset(end).ToUnixTimeSeconds();

        var query = ExternalApiQueryStore.Igdb.GenerateGamesReleasedBetweenQuery(startUnix, endUnix);
        var request = CreateIgdbRequest(query, "games");

        try
        {
            var igdbGames = await SendIgdbRequestAsync<IgdbGameReleaseDto>(request, cancellationToken);

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
                    g.Platforms != null &&
                    g.Platforms.Any() &&
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

            return _mapper.Map<List<GameReleaseResponse>>(filteredGames);
        }
        catch (IgdbApiException ex)
        {
            _logger.LogError(ex, "Erro ao se comunicar com a IGDB.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar lançamentos de jogos.");
            throw;
        }
    }
}