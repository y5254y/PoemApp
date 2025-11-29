// UpdateVipStatusDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdateVipStatusDto
{
    [Required]
    public DateTime VipStartDate { get; set; }

    [Required]
    public DateTime VipEndDate { get; set; }
}