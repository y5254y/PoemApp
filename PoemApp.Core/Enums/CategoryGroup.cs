using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

public enum CategoryGroup
{
    [Display(Name = "文体")]
    LiteraryForm,

    [Display(Name = "朝代")]
    Dynasty,

    [Display(Name = "题材")]
    Theme,

    [Display(Name = "教学阶段")]
    EducationLevel,

    [Display(Name = "风格")]
    Style



}
