namespace PoemApp.Core.DTOs;

public class CreateReviewDto
{
    public int RecitationId { get; set; }
    public int QualityRating { get; set; }
    public string? Notes { get; set; }
}
