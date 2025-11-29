using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums
{
    public enum DynastyEnum
    {
        [Display(Name = "先秦")]
        PreQin,

        [Display(Name = "秦")]
        Qin,
        [Display(Name = "汉")]
        Han,

        [Display(Name = "魏晋南北朝")]
        WeiJinNorthernAndSouthern,

        [Display(Name = "隋")]
        Sui,

        [Display(Name = "唐")]
        Tang,

        [Display(Name = "五代十国")]
        FiveDynasties,

        [Display(Name = "宋")]
        Song,

        [Display(Name = "元")]
        Yuan,

        [Display(Name = "明")]
        Ming,

        [Display(Name = "清")]
        Qing,

        [Display(Name = "近现代")]
        Modern
    }
}