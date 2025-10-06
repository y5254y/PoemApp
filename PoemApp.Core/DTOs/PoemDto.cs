using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class PoemDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }

    public string Background { get; set; }
    public string Translation { get; set; }
    public string Annotation { get; set; }
    public List<string> Categories { get; set; } = new List<string>();
}