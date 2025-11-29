// UserDto.cs
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? WeChatId { get; set; }
    public string? QQId { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; }
    public string RoleDisplayName { get; set; } = null!;
    public DateTime? VipStartDate { get; set; }
    public DateTime? VipEndDate { get; set; }
    public bool IsVip { get; set; }
    public int Points { get; set; }
}