using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdateCategoryDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Group { get; set; }

    public int? ParentId { get; set; }
}