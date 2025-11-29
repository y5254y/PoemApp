using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Enums
{
    public enum RelationshipTypeEnum
    {
        [Display(Name = "师徒")]
        TeacherStudent,

        [Display(Name = "朋友")]
        Friends,

        [Display(Name = "亲属")]
        Family,

        [Display(Name = "同僚")]
        Colleagues,

        [Display(Name = "影响")]
        Influence,

        [Display(Name = "竞争")]
        Rivals
    }
}