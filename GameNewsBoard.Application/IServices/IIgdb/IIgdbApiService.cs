using GameNewsBoard.Application.DTOs;

namespace GameNewsBoard.Application.IServices.Igdb
{
    public interface IIgdbApiService
    {
        Task<List<RawGameDto>> GetRawGamesAsync(string query, CancellationToken cancellationToken = default);
    }
}