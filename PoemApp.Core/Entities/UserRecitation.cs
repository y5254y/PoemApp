using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

/// <summary>
/// 用户背诵记录 - 记录用户开始背诵某首诗文的信息
/// </summary>
public class UserRecitation
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// 背诵状态
    /// </summary>
    [Required]
    public RecitationStatus Status { get; set; } = RecitationStatus.Learning;

    /// <summary>
    /// 首次背诵时间
    /// </summary>
    public DateTime FirstRecitationTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后一次复习时间
    /// </summary>
    public DateTime? LastReviewTime { get; set; }

    /// <summary>
    /// 复习次数
    /// </summary>
    public int ReviewCount { get; set; } = 0;

    /// <summary>
    /// 熟练度（0-100）- 可根据复习情况计算
    /// </summary>
    public int Proficiency { get; set; } = 0;

    /// <summary>
    /// 下次复习时间（根据艾宾浩斯曲线计算）
    /// </summary>
    public DateTime? NextReviewTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 该背诵记录的所有复习记录
    /// </summary>
    public ICollection<RecitationReview> Reviews { get; set; } = [];
}
