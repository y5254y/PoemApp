using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums;

public enum UserRole
{
    [Display(Name = "普通用户")]
    Normal,

    [Display(Name = "VIP用户")]
    Vip,

    [Display(Name = "管理员")]
    Admin,

    [Display(Name = "超级管理员")]
    SuperAdmin
}