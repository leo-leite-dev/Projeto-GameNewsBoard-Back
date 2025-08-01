using GameNewsBoard.Domain.IStorage;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace GameNewsBoard.Infrastructure.Services;

public class SupabaseImageStorageService : IImageStorageService
{
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;
    private const string Bucket = "uploads";

    public SupabaseImageStorageService(IConfiguration configuration)
    {
        _supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
                       ?? configuration["Supabase:Url"]
                       ?? throw new InvalidOperationException("SUPABASE_URL não configurada.");

        _supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY")
                       ?? configuration["Supabase:Key"]
                       ?? throw new InvalidOperationException("SUPABASE_KEY não configurada.");
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);

        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var uploadUrl = $"{_supabaseUrl}/storage/v1/object/{Bucket}/{newFileName}";
        var response = await httpClient.PostAsync(uploadUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro no upload: {response.StatusCode} - {errorMessage}");
        }

        return $"{_supabaseUrl}/storage/v1/object/public/{Bucket}/{newFileName}";
    }

    public async Task DeleteImageAsync(string filePath)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);

        var deleteUrl = $"{_supabaseUrl}/storage/v1/object/{Bucket}/{filePath}";
        var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao remover imagem: {response.StatusCode} - {errorMessage}");
        }
    }
}