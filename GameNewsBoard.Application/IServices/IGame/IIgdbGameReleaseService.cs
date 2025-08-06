using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.IServices.IGame
{
    public interface IIgdbGameReleaseService
    {
        Task<List<GameReleaseResponse>> GetTodayReleasesGamesAsync(PlatformFamily? platform = null, CancellationToken cancellationToken = default);
        Task<List<GameReleaseResponse>> GetUpcomingGamesAsync(int daysAhead = 7, PlatformFamily? platform = null, CancellationToken cancellationToken = default);
        Task<List<GameReleaseResponse>> GetRecentlyReleasedGamesAsync(int daysBack = 7, PlatformFamily? platform = null, CancellationToken cancellationToken = default);
        Task<List<GameReleaseResponse>> GetReleasesBetweenAsync(DateTime start, DateTime end, PlatformFamily? platform = null, CancellationToken cancellationToken = default);
    }
}
