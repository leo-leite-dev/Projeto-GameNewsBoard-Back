using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.ISteam;
using GameNewsBoard.Domain.Commons;
using Microsoft.Extensions.Logging;

namespace GameNewsBoard.Application.Services
{
    public class SteamUserService : ISteamUserService
    {
        private readonly ISteamApiService _steamApiService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SteamUserService> _logger;

        public SteamUserService(
            ISteamApiService steamApiService,
            IUserRepository userRepository,
            ILogger<SteamUserService> logger)
        {
            _steamApiService = steamApiService ?? throw new ArgumentNullException(nameof(steamApiService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger;
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
            return await _steamApiService.GetSteamUserProfileAsync(steamId);
        }

        public async Task<SteamUserProfileResponse?> GetSteamUserProfileWithGamesAsync(Guid userId)
        {
            var profile = await GetCompleteSteamUserProfileAsync(userId);
            if (profile == null) return null;

            var steamId = profile.SteamId;
            var ownedGames = await _steamApiService.GetOwnedGamesAsync(steamId);

            var (stats, validGames) = await ProcessGamesAndCalculateStats(steamId, ownedGames);

            profile.Games = validGames;
            profile.TotalAchievements = stats.TotalAchievements;
            profile.CompletedGamesCount = stats.CompletedGamesCount;
            profile.AverageCompletionPercent = validGames.Count > 0
                ? stats.TotalCompletionPercentage / validGames.Count
                : 0;
            profile.LastUnlockedAchievement = stats.LastUnlockedAchievement;
            profile.RarestAchievement = null;

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

        private async Task<(SteamProfileStats Stats, List<OwnedGameResponse> ValidGames)> ProcessGamesAndCalculateStats(
            string steamId,
            List<OwnedGameResponse> games)
        {
            int totalAchievements = 0;
            int totalCompletedGames = 0;
            double totalCompletionPercentage = 0;

            SteamAchievementResponse? lastUnlocked = null;
            var validGames = new List<OwnedGameResponse>();

            foreach (var game in games)
            {
                _logger.LogDebug("[SteamProfile] Processando jogo: {GameName} (AppId: {AppId})", game.Name, game.AppId);

                var achievements = await _steamApiService.GetPlayerAchievementsAsync(steamId, game.AppId);

                if (achievements == null || achievements.Count == 0)
                {
                    _logger.LogDebug("[SteamProfile] Jogo ignorado por não possuir conquistas: {GameName} (AppId: {AppId})", game.Name, game.AppId);
                    continue;
                }

                game.Achievements = achievements;

                var unlockedCount = achievements.Count(a => a.IsUnlocked);
                var totalCount = achievements.Count;

                game.AchievementsUnlocked = unlockedCount;
                game.TotalAchievements = totalCount;

                if (totalCount > 0)
                {
                    double percent = (double)unlockedCount / totalCount;
                    totalCompletionPercentage += percent * 100;

                    if (unlockedCount == totalCount)
                        totalCompletedGames++;
                }

                UpdateLastAchievement(ref lastUnlocked, achievements);

                totalAchievements += unlockedCount;
                validGames.Add(game);
            }

            return (
                new SteamProfileStats(
                    totalAchievements,
                    totalCompletedGames,
                    totalCompletionPercentage,
                    lastUnlocked,
                    null // raridade desabilitada
                ),
                validGames
            );
        }

        private void UpdateLastAchievement(ref SteamAchievementResponse? current, List<SteamAchievementResponse> achievements)
        {
            var last = achievements
                .Where(a => a.IsUnlocked && a.UnlockedAt.HasValue)
                .OrderByDescending(a => a.UnlockedAt!.Value)
                .FirstOrDefault();

            if (last != null && (current == null || last.UnlockedAt > current.UnlockedAt))
            {
                current = last;
            }
        }
    }
}