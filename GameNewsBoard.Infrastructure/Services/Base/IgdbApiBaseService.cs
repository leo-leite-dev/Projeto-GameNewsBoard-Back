using GameNewsBoard.Application.Exceptions.Api;
using GameNewsBoard.Infrastructure.Helpers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GameNewsBoard.Infrastructure.External.IGDB.Base
{
    public abstract class IgdbApiBaseService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _clientId;
        protected readonly string _accessToken;

        protected IgdbApiBaseService(HttpClient httpClient, string clientId, string accessToken)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        }

        protected HttpRequestMessage CreateIgdbRequest(string query, string endpoint)
        {
            var url = IgdbApiUrlBuilder.BuildIgdbUrl(endpoint);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Client-ID", _clientId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            request.Content = new StringContent(query, Encoding.UTF8, "text/plain");
            return request;

        }
        protected async Task<T> SendIgdbRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new IgdbApiException($"IGDB error: {response.StatusCode}, Response: {content}");

                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new IgdbApiException("Resposta vazia da IGDB.");
            }
            catch (HttpRequestException ex)
            {
                throw new IgdbApiException("Network error when accessing IGDB.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new IgdbApiException("Request timeout when accessing IGDB.", ex);
            }
            catch (Exception ex)
            {
                throw new IgdbApiException("Unexpected error when accessing IGDB.", ex);
            }
        }
    }
}