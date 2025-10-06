using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdatePoemDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int AuthorId { get; set; }

    public string Background { get; set; }
    public string Translation { get; set; }
    public string Annotation { get; set; }
    public List<int> CategoryIds { get; set; } = new List<int>();
}