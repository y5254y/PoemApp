using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

public enum CategoryTypeEnum
{
    [Display(Name = "小学")]
    PrimarySchool,

    [Display(Name = "初中")]
    MiddleSchool,

    [Display(Name = "高中")]
    HighSchool,

    [Display(Name = "大学")]
    University,

    [Display(Name = "唐诗")]
    TangPoetry,

    [Display(Name = "宋词")]
    SongCi,

    [Display(Name = "元曲")]
    YuanQu,

    [Display(Name = "诗经")]
    BookOfSongs,

    [Display(Name = "楚辞")]
    ChuCi,

    [Display(Name = "乐府")]
    YueFu
}