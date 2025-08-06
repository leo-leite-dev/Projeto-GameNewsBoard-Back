using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Application.DTOs
{
    public record RawGameReleaseDto
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public GameCategory? Category { get; init; }
        public string CoverImageUrl { get; init; } = string.Empty;
        public long? ReleaseDateUnix { get; init; }
        public List<string> Platforms { get; init; } = new();
    }
}