// UpdateUserDto.cs
using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UpdateUserDto
{
    [StringLength(50)]
    public string? Username { get; set; }

    [StringLength(50)]
    public string? WeChatId { get; set; }

    [StringLength(50)]
    public string? QQId { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public UserRole? Role { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "密码与确认密码不一致")]
    public string? ConfirmPassword { get; set; }
}