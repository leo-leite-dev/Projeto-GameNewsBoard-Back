using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.DTOs.Responses.Game
{
    public class GameStatusResponse
    {
        public int GameId { get; set; }
        public GameResponse Game { get; set; } = null!;
        public Status Status { get; set; }
    }
}