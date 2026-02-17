using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

/// <summary>
/// 反馈状态
/// </summary>
public enum FeedbackStatus
{
    [Display(Name = "待审核")]
    Pending = 0,

    [Display(Name = "已处理")]
    Resolved = 1,

    [Display(Name = "已拒绝")]
    Rejected = 2
}
