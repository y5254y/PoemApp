// UserFavoriteDto.cs
namespace PoemApp.Core.DTOs;

public class UserFavoriteDto
{
    public int UserId { get; set; }
    public int PoemId { get; set; }
    public string PoemTitle { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime FavoriteTime { get; set; }
}