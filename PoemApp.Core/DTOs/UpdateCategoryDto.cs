using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UpdateCategoryDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public CategoryGroup? Group { get; set; }

    public int? ParentId { get; set; }
}