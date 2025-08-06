namespace GameNewsBoard.Application.DTOs.Responses.Game
{
    public class GNewsResponse
    {
        public int TotalResults { get; set; }
        public List<GNewsArticleResponse> Articles { get; set; } = new();
    }

    public class GNewsArticleResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PubDate { get; set; }
    }
}