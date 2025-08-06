using GameNewsBoard.Application.DTOs.Responses.Game;

namespace GameNewsBoard.Application.IServices.IGame
{
    public interface IGNewsService
    {
        Task<GNewsResponse> GetLatestNewsAsync(string platform);
    }
}
