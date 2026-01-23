using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

/// <summary>
/// 成就定义 - 定义所有可能的成就
/// </summary>
public class Achievement
{
    public int Id { get; set; }

    /// <summary>
    /// 成就名称
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 成就描述
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = null!;

    /// <summary>
    /// 成就类型
    /// </summary>
    [Required]
    public AchievementType Type { get; set; }

    /// <summary>
    /// 成就图标URL或路径
    /// </summary>
    [StringLength(200)]
    public string? IconUrl { get; set; }

    /// <summary>
    /// 达成条件值（如：连续签到7天，这里就是7）
    /// </summary>
    public int TargetValue { get; set; }

    /// <summary>
    /// 奖励积分
    /// </summary>
    public int RewardPoints { get; set; } = 0;

    /// <summary>
    /// 成就等级（用于同类成就的不同级别，如：铜、银、金）
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// 是否隐藏（隐藏成就在达成前不显示）
    /// </summary>
    public bool IsHidden { get; set; } = false;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 获得该成就的用户列表
    /// </summary>
    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}
