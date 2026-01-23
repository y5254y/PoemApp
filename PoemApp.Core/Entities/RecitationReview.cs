using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

/// <summary>
/// 复习记录 - 基于艾宾浩斯遗忘曲线的复习计划和完成情况
/// 艾宾浩斯遗忘曲线复习时间点：1天、2天、4天、7天、15天、30天
/// </summary>
public class RecitationReview
{
    public int Id { get; set; }

    [Required]
    public int UserRecitationId { get; set; }
    public UserRecitation UserRecitation { get; set; } = null!;

    /// <summary>
    /// 计划复习时间
    /// </summary>
    public DateTime ScheduledTime { get; set; }

    /// <summary>
    /// 实际复习时间
    /// </summary>
    public DateTime? ActualReviewTime { get; set; }

    /// <summary>
    /// 复习状态
    /// </summary>
    [Required]
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

    /// <summary>
    /// 复习轮次（第几次复习：1、2、3...）
    /// </summary>
    public int ReviewRound { get; set; }

    /// <summary>
    /// 复习质量评分（1-5）：1=完全忘记，2=模糊，3=能想起，4=熟练，5=完美
    /// </summary>
    public int? QualityRating { get; set; }

    /// <summary>
    /// 复习耗时（秒）
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// 复习备注
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 是否发送了提醒
    /// </summary>
    public bool ReminderSent { get; set; } = false;

    /// <summary>
    /// 提醒发送时间
    /// </summary>
    public DateTime? ReminderSentTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
