using GameNewsBoard.Application.DTOs.Base;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.Responses.DTOs.Responses
{
    public class GameReleaseResponse : GameBaseDto
    {
        public GameCategory? Category { get; set; }
    }
}