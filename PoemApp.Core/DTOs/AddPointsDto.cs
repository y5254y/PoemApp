// AddPointsDto.cs
using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class AddPointsDto
{
    [Required]
    public PointsSource Source { get; set; }

    [Required]
    public int Points { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public int? RelatedId { get; set; }
}