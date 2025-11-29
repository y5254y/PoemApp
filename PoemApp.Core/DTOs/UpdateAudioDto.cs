// UpdateAudioDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class UpdateAudioDto
{
    [StringLength(255)]
    public string? FileUrl { get; set; }
}