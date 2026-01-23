// DetailedUserDto.cs
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class DetailedUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? WeChatId { get; set; }
    public string? QQId { get; set; }
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VipStartDate { get; set; }
    public DateTime? VipEndDate { get; set; }
    public int Points { get; set; }

    public List<PoemDto> Favorites { get; set; } = new();
    public List<RecitationDto> Recitations { get; set; } = new();
    public List<UserAchievementDto> Achievements { get; set; } = new();
}