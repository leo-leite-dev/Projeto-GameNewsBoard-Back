using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.DTOs
{
    public class UserProfileDto
    {
        public User User { get; set; } = null!;
        public List<TierList> Tiers { get; set; } = new();
        public List<GameStatus> GameStatuses { get; set; } = new();
    }
}