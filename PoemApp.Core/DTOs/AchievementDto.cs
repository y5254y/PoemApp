namespace PoemApp.Core.DTOs;

public class AchievementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int TargetValue { get; set; }
    public int RewardPoints { get; set; }
    public int Level { get; set; }
    public bool IsHidden { get; set; }
}
