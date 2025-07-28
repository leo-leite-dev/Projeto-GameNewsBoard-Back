namespace GameNewsBoard.Application.DTOs.Base
{
    public class GameBaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
    }
}