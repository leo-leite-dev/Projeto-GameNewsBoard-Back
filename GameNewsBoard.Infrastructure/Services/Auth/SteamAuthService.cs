using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.Auth;
using GameNewsBoard.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GameNewsBoard.Infrastructure.Services.Auth
{
    public class SteamAuthService : ISteamAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SteamAuthService> _logger;

        public SteamAuthService(IUserRepository userRepository, ILogger<SteamAuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> AuthenticateOrCreateSteamUserAsync(string steamId)
        {
            var existingUser = (await _userRepository.FindAsync(u => u.SteamId == steamId)).FirstOrDefault();

            if (existingUser != null)
            {
                _logger.LogInformation("SteamId {SteamId} já está vinculado ao usuário {Username}.", steamId, existingUser.Username);
                return existingUser;
            }

            var newUser = new User($"usuario_{Guid.NewGuid().ToString()[..8]}", "")
            {
                SteamId = steamId
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Novo usuário criado com SteamId {SteamId}", steamId);
            return newUser;
        }
    }
}