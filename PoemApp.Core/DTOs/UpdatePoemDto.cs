using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UpdatePoemDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public DynastyEnum Dynasty { get; set; }

    public string Background { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Annotation { get; set; } = string.Empty;
    public string Appreciation { get; set; } = string.Empty;
    public List<int> CategoryIds { get; set; } = new List<int>();
}