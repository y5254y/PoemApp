// AudioRatingDto.cs
namespace PoemApp.Core.DTOs;

public class AudioRatingDto
{
    public int Id { get; set; }
    public int AudioId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingTime { get; set; }
}