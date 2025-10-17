using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UpdateCategoryDto
{
    [Required]
    public CategoryTypeEnum Type { get; set; }

    public string? Description { get; set; }
}