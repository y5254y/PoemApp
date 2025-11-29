// CreateAudioDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class CreateAudioDto
{
    [Required]
    public int PoemId { get; set; }

    [Required]
    [StringLength(255)]
    public string FileUrl { get; set; } = null!;

    public int? UploaderId { get; set; }
}