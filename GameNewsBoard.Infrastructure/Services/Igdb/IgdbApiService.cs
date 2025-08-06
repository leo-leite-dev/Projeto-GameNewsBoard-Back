using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.IServices.Igdb;
using GameNewsBoard.Infrastructure.Configurations.Settings;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos;
using GameNewsBoard.Infrastructure.Service.IGDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsBoard.Infrastructure.Services.Igdb
{
    public class IgdbApiService : IgdbApiBaseService, IIgdbApiService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<IgdbApiService> _logger;

        public IgdbApiService(
            HttpClient httpClient,
            IOptions<IgdbSettings> settings,
            IMapper mapper,
            ILogger<IgdbApiService> logger)
            : base(httpClient, settings.Value.ClientId, settings.Value.AccessToken)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<RawGameDto>> GetRawGamesAsync(string query, CancellationToken cancellationToken = default)
        {
            var request = CreateIgdbRequest(query, "games");

            try
            {
                var igdbGames = await SendIgdbRequestAsync<List<IgdbGameDto>>(request, cancellationToken);

                return _mapper.Map<List<RawGameDto>>(igdbGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter jogos da IGDB com query: {Query}", query);
                throw;
            }
        }
    }
}