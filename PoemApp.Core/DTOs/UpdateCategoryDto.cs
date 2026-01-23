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
    public int SortOrder { get; set; } = 0;
    public bool IsEnabled { get; set; } = true;
    public bool IsLeaf { get; set; } = true;
}