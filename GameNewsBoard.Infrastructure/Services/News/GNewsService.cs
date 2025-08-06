using System.Text.Json;
using AutoMapper;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.IServices.IGame;
using GameNewsBoard.Infrastructure.Configurations.Settings;
using GameNewsBoard.Infrastructure.ExternalDtos;
using GameNewsBoard.Infrastructure.Helpers;
using Microsoft.Extensions.Options;


namespace GameNewsBoard.Infrastructure.Services.News
{
    public class GNewsService : IGNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GNewsService(
            HttpClient httpClient,
            IMapper mapper,
            IOptions<GNewsSettings> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _apiKey = settings.ApiKey ?? throw new InvalidOperationException("API Key não configurada");
            _baseUrl = settings.BaseUrl ?? throw new InvalidOperationException("BaseUrl não configurada");
        }

        public async Task<GNewsResponse> GetLatestNewsAsync(string platform)
        {
            var url = IgdbApiUrlBuilder.BuildNewsApiUrl(_baseUrl, _apiKey, platform);

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro da API GNews: {response.StatusCode} - {content}");

            var gnewsData = JsonSerializer.Deserialize<GNewsResponseWrapper>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (gnewsData is null || gnewsData.Articles.Count == 0)
                return new GNewsResponse();

            return _mapper.Map<GNewsResponse>(gnewsData);
        }
    }
}