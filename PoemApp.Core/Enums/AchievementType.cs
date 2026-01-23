using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

/// <summary>
/// 成就类型
/// </summary>
public enum AchievementType
{
    [Display(Name = "连续签到")]
    ContinuousCheckIn,

    [Display(Name = "背诵诗文")]
    RecitationCount,

    [Display(Name = "收藏诗文")]
    FavoriteCount,

    [Display(Name = "上传音频")]
    AudioUpload,

    [Display(Name = "获得评分")]
    RatingReceived,

    [Display(Name = "添加标注")]
    AnnotationCount,

    [Display(Name = "积分累计")]
    PointsTotal,

    [Display(Name = "使用天数")]
    UsageDays,

    [Display(Name = "学习时长")]
    StudyDuration,

    [Display(Name = "完美复习")]
    PerfectReview,

    [Display(Name = "特殊成就")]
    Special
}
