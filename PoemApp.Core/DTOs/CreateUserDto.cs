// CreateUserDto.cs
using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(50)]
    public string? WeChatId { get; set; }

    [StringLength(50)]
    public string? QQId { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public UserRole Role { get; set; } = UserRole.Normal;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = null!;
}