using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.DTOs.Responses.TierList
{
    public class TierListEntryResponse
    {
        public int GameId { get; set; }
        public GameResponse Game { get; set; } = null!;
        public TierLevel Tier { get; set; }
    }
}