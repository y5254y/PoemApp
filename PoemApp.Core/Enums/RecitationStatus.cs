using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

/// <summary>
/// 背诵状态
/// </summary>
public enum RecitationStatus
{
    [Display(Name = "学习中")]
    Learning,

    [Display(Name = "已掌握")]
    Mastered,

    [Display(Name = "需要复习")]
    NeedReview,

    [Display(Name = "已放弃")]
    Abandoned
}
