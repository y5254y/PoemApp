using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class CreateQuoteFeedbackDto
{
    [Required]
    public int QuoteId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = null!;

    public FeedbackCategory? Category { get; set; }
}
