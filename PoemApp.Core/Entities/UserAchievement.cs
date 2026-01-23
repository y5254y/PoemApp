using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

/// <summary>
/// 用户成就记录 - 记录用户获得的成就
/// </summary>
public class UserAchievement
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public int AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;

    /// <summary>
    /// 获得成就的时间
    /// </summary>
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 当前进度值（用于显示进度条）
    /// </summary>
    public int CurrentValue { get; set; }

    /// <summary>
    /// 是否已领取奖励
    /// </summary>
    public bool RewardClaimed { get; set; } = false;

    /// <summary>
    /// 奖励领取时间
    /// </summary>
    public DateTime? RewardClaimedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
