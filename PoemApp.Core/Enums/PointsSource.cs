using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

public enum PointsSource
{
    [Display(Name = "每日签到")]
    DailyCheckIn,

    [Display(Name = "完成任务")]
    TaskCompletion,

    [Display(Name = "上传音频")]
    AudioUpload,

    [Display(Name = "音频被评分")]
    AudioRated,

    [Display(Name = "添加标注")]
    AnnotationAdded,

    [Display(Name = "收藏诗文")]
    PoemFavorited,

    [Display(Name = "管理员调整")]
    AdminAdjustment,

    [Display(Name = "活动奖励")]
    ActivityReward,

    [Display(Name = "连续签到奖励")]
    ContinuousCheckInReward,

    [Display(Name = "背诵诗文")]
    RecitationCompleted,

    [Display(Name = "完成复习")]
    ReviewCompleted,

    [Display(Name = "获得成就")]
    AchievementUnlocked
}