using GameNewsBoard.Application.DTOs.Base;

namespace GameNewsBoard.Application.DTOs.Responses
{
    public class GameResponse : GameBaseDto
    {
        public double Rating { get; set; }
    }
}