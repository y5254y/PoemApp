using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class BasicUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? WeChatId { get; set; }
    public string? QQId { get; set; }
    public string? Phone { get; set; }
    public int Points { get; set; }
    public string Role { get; set; } = null!;
    public string RoleDisplayName { get; set; } = null!;
}