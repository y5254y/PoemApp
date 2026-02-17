using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdateFeedbackStatusDto
{
    [Required]
    public FeedbackStatus Status { get; set; }

    [StringLength(500)]
    public string? AdminReply { get; set; }
}
