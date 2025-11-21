// AudioDto.cs
namespace PoemApp.Core.DTOs;

public class AudioDto
{
    public int Id { get; set; }
    public int PoemId { get; set; }
    public string PoemTitle { get; set; } = null!;
    public string? FileUrl { get; set; } 
    public int? UploaderId { get; set; }
    public string? UploaderName { get; set; }
    public DateTime UploadTime { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
}