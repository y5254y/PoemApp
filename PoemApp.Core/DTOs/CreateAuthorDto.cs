using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class CreateAuthorDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    public DynastyEnum Dynasty { get; set; }

    public string? Biography { get; set; }
}