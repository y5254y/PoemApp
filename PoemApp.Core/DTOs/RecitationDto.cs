namespace PoemApp.Core.DTOs;

public class RecitationDto
{
    public int Id { get; set; }
    public int PoemId { get; set; }
    public string PoemTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? NextReviewTime { get; set; }
    public int Proficiency { get; set; }
}
