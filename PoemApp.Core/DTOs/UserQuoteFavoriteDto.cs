namespace PoemApp.Core.DTOs;

public class UserQuoteFavoriteDto
{
    public int UserId { get; set; }
    public int QuoteId { get; set; }
    public string QuoteContent { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public DateTime FavoriteTime { get; set; }
}
