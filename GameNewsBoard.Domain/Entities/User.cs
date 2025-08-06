namespace GameNewsBoard.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Username { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public string? SteamId { get; set; }

        public ICollection<TierList> TierLists { get; private set; } = new List<TierList>();
        public ICollection<GameStatus> GameStatuses { get; private set; } = new List<GameStatus>();

        private User() { }

        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
