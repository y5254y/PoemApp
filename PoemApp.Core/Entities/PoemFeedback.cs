using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class PoemFeedback
{
    public int Id { get; set; }

    [Required]
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = null!;

    public FeedbackCategory? Category { get; set; }

    public FeedbackStatus Status { get; set; } = FeedbackStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    [StringLength(500)]
    public string? AdminReply { get; set; }
}
