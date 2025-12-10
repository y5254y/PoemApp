using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class QuoteDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Content { get; set; } = string.Empty;

    public int? AuthorId { get; set; }
    public string? AuthorName { get; set; }

    public int? PoemId { get; set; }
    public string? PoemTitle { get; set; }

    [StringLength(200)]
    public string? Source { get; set; }

    public string? Translation { get; set; }

    public string? Annotation { get; set; }

    public DateTime CreatedAt { get; set; }
}