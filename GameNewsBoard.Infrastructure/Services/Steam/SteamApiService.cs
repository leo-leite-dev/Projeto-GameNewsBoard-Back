using System.Text.Json;
using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Application.IServices.ISteam;
using GameNewsBoard.Infrastructure.Configurations.Settings;
using GameNewsBoard.Infrastructure.External.Igdb.Steam;
using GameNewsBoard.Infrastructure.External.Steam.Dtos;
using GameNewsBoard.Infrastructure.ExternalDtos.Steam;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsBoard.Infrastructure.Services.Steam
{
    public class SteamApiService : ISteamApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IMapper _mapper;
        private readonly ILogger<SteamApiService> _logger;

        public SteamApiService(
            HttpClient httpClient,
             IOptions<SteamSettings> steamOptions,
             IMapper mapper,
             ILogger<SteamApiService> logger
             )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = steamOptions?.Value.ApiKey ?? throw new ArgumentNullException(nameof(steamOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SteamUserProfileResponse?> GetSteamUserProfileAsync(string steamId)
        {
            var url = SteamApiUrlBuilder.GetPlayerSummaryUrl(_apiKey, steamId);

            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao obter perfil da Steam. SteamId: {SteamId}, StatusCode: {StatusCode}, URL: {Url}, Conteúdo: {Content}",
                        steamId, response.StatusCode, url, content);
                    return null;
                }

                var dto = JsonSerializer.Deserialize<SteamPlayerResponseDto>(content);
                var player = dto?.Response.Players.FirstOrDefault();

                if (player == null)
                {
                    _logger.LogWarning("Perfil Steam não encontrado para SteamId: {SteamId}. URL: {Url}", steamId, url);
                    return null;
                }

                return _mapper.Map<SteamUserProfileResponse>(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil da Steam para SteamId: {SteamId}. URL: {Url}", steamId, url);
                return null;
            }
        }

        public async Task<List<OwnedGameResponse>> GetOwnedGamesAsync(string steamId)
        {
            try
            {
                var url = SteamApiUrlBuilder.GetOwnedGamesUrl(_apiKey, steamId);
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<SteamOwnedGamesResponseDto>(json);

                var rawGames = _mapper.Map<List<RawSteamGameDto>>(dto.Response.Games);
                var result = _mapper.Map<List<OwnedGameResponse>>(rawGames);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter jogos da Steam para SteamId: {SteamId}", steamId);
                return new();
            }
        }

        public async Task<List<SteamAchievementResponse>> GetPlayerAchievementsAsync(string steamId, int appId)
        {
            var url = SteamApiUrlBuilder.GetPlayerAchievementsUrl(_apiKey, steamId, appId);

            try
            {
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Jogo sem conquistas ou erro na API. SteamId: {SteamId}, AppId: {AppId}, StatusCode: {StatusCode}, Url: {Url}",
                        steamId, appId, response.StatusCode, url);
                    return new();
                }

                var dto = JsonSerializer.Deserialize<SteamPlayerAchievementsResponseDto>(json);
                return _mapper.Map<List<SteamAchievementResponse>>(dto?.PlayerStats?.Achievements) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter conquistas. SteamId: {SteamId}, AppId: {AppId}, Url: {Url}", steamId, appId, url);
                return new();
            }
        }

        public async Task<Dictionary<string, double>> GetGlobalAchievementPercentagesAsync(int appId)
        {
            var url = SteamApiUrlBuilder.GetGlobalAchievementsUrl(_apiKey, appId);

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<SteamGlobalAchievementsResponseDto>(json);

                var list = dto?.Response.Achievementpercentages ?? new();
                return list.ToDictionary(a => a.Name, a => a.Percent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conquistas globais para AppId: {AppId}", appId);
                return new();
            }
        }
    }
}