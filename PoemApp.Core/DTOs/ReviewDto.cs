namespace PoemApp.Core.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public DateTime ScheduledTime { get; set; }
    public DateTime? ActualReviewTime { get; set; }
    public string Status { get; set; } = null!;
    public int? QualityRating { get; set; }
    public int ReviewRound { get; set; }
    // The user recitation id this review belongs to
    public int UserRecitationId { get; set; }

    // Associated poem information to allow frontend to show context
    public int PoemId { get; set; }
    public string? PoemTitle { get; set; }
}
