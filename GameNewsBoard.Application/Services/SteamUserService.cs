using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.ISteam;
using GameNewsBoard.Domain.Commons;

namespace GameNewsBoard.Application.Services
{
    public class SteamUserService : ISteamUserService
    {
        private readonly ISteamApiService _steamApiService;
        private readonly IUserRepository _userRepository;

        public SteamUserService(
            ISteamApiService steamApiService,
            IUserRepository userRepository
            )
        {
            _steamApiService = steamApiService ?? throw new ArgumentNullException(nameof(steamApiService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<SteamUserProfileResponse?> GetCompleteSteamUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApplicationException("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(user.SteamId))
                throw new ApplicationException("O usuário ainda não vinculou sua conta Steam.");

            return await GetCompleteSteamUserProfileAsync(user.SteamId);
        }

        public async Task<SteamUserProfileResponse?> GetCompleteSteamUserProfileAsync(string steamId)
        {
            var profile = await _steamApiService.GetSteamUserProfileAsync(steamId);
            return profile; 
        }

        public async Task<Result> LinkSteamAccountAsync(Guid userId, string steamId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("Usuário não encontrado.");

            if (!string.IsNullOrWhiteSpace(user.SteamId))
                return Result.Failure("O usuário já vinculou uma conta Steam.");

            var steamProfile = await _steamApiService.GetSteamUserProfileAsync(steamId);
            if (steamProfile == null)
                return Result.Failure("SteamId inválido ou não foi possível obter dados da Steam.");

            user.SteamId = steamId;
            await _userRepository.UpdateAsync(user);

            return Result.Success();
        }
    }
}