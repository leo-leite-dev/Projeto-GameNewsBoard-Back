using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Domain.IStorage;
using GameNewsBoard.Infrastructure.Helpers;
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

    public string BasePublicUrl => $"{_supabaseUrl}/storage/v1/object/public/{Bucket}";

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, Guid userId, ImageBucketCategory category)
    {
        var sanitizedCategory = category.ToString().ToLower();
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var storagePath = $"{userId}/file/{sanitizedCategory}/{newFileName}";

        using var httpClient = new HttpClient();
        SetHeaders(httpClient);

        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", storagePath);

        var uploadUrl = SupabaseUrlBuilder.BuildUploadUrl(_supabaseUrl, Bucket, storagePath);
        var response = await httpClient.PostAsync(uploadUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro no upload: {response.StatusCode} - {errorMessage}");
        }

        return SupabaseUrlBuilder.BuildPublicUrl(_supabaseUrl, Bucket, storagePath);
    }

    public async Task DeleteImageAsync(string filePath)
    {
        var uploadsIndex = filePath.IndexOf($"{Bucket}/");
        var relativePath = uploadsIndex >= 0 ? filePath.Substring(uploadsIndex + Bucket.Length + 1) : filePath;

        using var httpClient = new HttpClient();
        SetHeaders(httpClient);

        var deleteUrl = SupabaseUrlBuilder.BuildDeleteUrl(_supabaseUrl, Bucket, relativePath);
        var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao remover imagem: {response.StatusCode} - {errorMessage}");
        }
    }

    private void SetHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
    }
}