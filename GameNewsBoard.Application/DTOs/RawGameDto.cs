namespace GameNewsBoard.Application.DTOs
{
    public record RawGameDto
    {
        public string Title { get; init; } = string.Empty;
        public float? Rating { get; init; }
        public long? ReleaseDateUnix { get; init; }
        public string CoverImageUrl { get; init; } = string.Empty;
        public List<string> Platforms { get; init; } = new();
    }
}
