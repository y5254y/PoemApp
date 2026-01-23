namespace PoemApp.Core.DTOs;

public class UserAchievementDto
{
    public int Id { get; set; }
    public string AchievementName { get; set; } = null!;
    public DateTime AchievedAt { get; set; }
    public int CurrentValue { get; set; }
    public bool RewardClaimed { get; set; }
}
