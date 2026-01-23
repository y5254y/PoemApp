using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

/// <summary>
/// 复习状态
/// </summary>
public enum ReviewStatus
{
    [Display(Name = "待复习")]
    Pending,

    [Display(Name = "已完成")]
    Completed,

    [Display(Name = "已跳过")]
    Skipped,

    [Display(Name = "已过期")]
    Expired
}
