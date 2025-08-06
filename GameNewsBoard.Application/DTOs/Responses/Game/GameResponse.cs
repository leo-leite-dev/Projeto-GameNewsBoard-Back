using GameNewsBoard.Application.DTOs.Base;

namespace GameNewsBoard.Application.DTOs.Responses.Game
{
    public class GameResponse : GameBaseDto
    {
        public double Rating { get; set; }
    }
}