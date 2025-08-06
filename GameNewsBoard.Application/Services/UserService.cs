using AutoMapper;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.Settings;
using Microsoft.Extensions.Options;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses.User;
using GameNewsBoard.Application.IServices.ISteam;

namespace GameNewsBoard.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ISteamUserService _steamUserService;
        private readonly IUserRepository _userRepository;
        private readonly ITierListRepository _tierListRepository;
        private readonly IGameStatusRepository _gameStatusRepository;
        private readonly IMapper _mapper;
        private readonly BackendSettings _backendSettings;

        public UserService(
            ISteamUserService steamUserService,
            IUserRepository userRepository,
            ITierListRepository tierListRepository,
            IGameStatusRepository gameStatusRepository,
            IMapper mapper,
            IOptions<BackendSettings> backendSettings)
        {
            _steamUserService = steamUserService ?? throw new ArgumentNullException(nameof(steamUserService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tierListRepository = tierListRepository ?? throw new ArgumentNullException(nameof(tierListRepository));
            _gameStatusRepository = gameStatusRepository ?? throw new ArgumentNullException(nameof(gameStatusRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _backendSettings = backendSettings?.Value ?? throw new ArgumentNullException(nameof(backendSettings));
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApplicationException("Usuário não encontrado.");

            var tierLists = await _tierListRepository.GetByUserIdAsync(userId);
            var gameStatuses = await _gameStatusRepository.GetByUserIdAsync(userId);

            var dto = new UserProfileDto
            {
                User = user,
                Tiers = tierLists.ToList(),
                GameStatuses = gameStatuses.ToList()
            };

            var response = _mapper.Map<UserProfileResponse>(dto);

            foreach (var tier in response.Tiers)
            {
                tier.ImageUrl = $"{_backendSettings.Url}{tier.ImageUrl}";
            }

            if (!string.IsNullOrEmpty(user.SteamId))
                response.SteamProfile = await _steamUserService.GetCompleteSteamUserProfileAsync(user.SteamId);

            return response;
        }
    }
}