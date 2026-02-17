using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

/// <summary>
/// 反馈分类
/// </summary>
public enum FeedbackCategory
{
    [Display(Name = "错别字")]
    Typo = 0,

    [Display(Name = "翻译错误")]
    TranslationError = 1,

    [Display(Name = "注释问题")]
    AnnotationIssue = 2,

    [Display(Name = "信息不完整")]
    IncompleteInfo = 3,

    [Display(Name = "其他")]
    Other = 4
}
