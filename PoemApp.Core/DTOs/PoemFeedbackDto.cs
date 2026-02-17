using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class PoemFeedbackDto
{
    public int Id { get; set; }

    public int PoemId { get; set; }

    public string? PoemTitle { get; set; }

    public int UserId { get; set; }

    public string? Username { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = null!;

    public FeedbackCategory? Category { get; set; }

    public FeedbackStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? AdminReply { get; set; }
}
