namespace GameNewsBoard.Application.DTOs.Responses
{
public record UploadedImageResponse
{
    public Guid ImageId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsUsed { get; init; }
}
}