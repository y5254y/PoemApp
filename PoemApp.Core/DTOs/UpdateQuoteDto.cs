using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdateQuoteDto
{
    [Required]
    [StringLength(500)]
    public string Content { get; set; } = string.Empty;

    public int? AuthorId { get; set; }
    public int? PoemId { get; set; }

    [StringLength(200)]
    public string? Source { get; set; }

    public string? Translation { get; set; }

    public string? Annotation { get; set; }
}