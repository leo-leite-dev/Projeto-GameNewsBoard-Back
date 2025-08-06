using GameNewsBoard.Application.DTOs.Base;
using GameNewsBoard.Domain.Enums;

namespace  GameNewsBoard.Application.DTOs.Responses.Game
{
    public class GameReleaseResponse : GameBaseDto
    {
        public GameCategory? Category { get; set; }
    }
}