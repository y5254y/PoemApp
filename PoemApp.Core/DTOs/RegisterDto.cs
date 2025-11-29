// RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    // 可选：如果需要密码注册
    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    public string? WeChatCode { get; set; }
    public string? QQCode { get; set; }
    public string? Phone { get; set; }
}