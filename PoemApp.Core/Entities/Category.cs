
using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;
using PoemApp.Core.Extensions;
namespace PoemApp.Core.Entities;

public class Category
{
    public int Id { get; set; }


    // 使用枚举类型
    public CategoryTypeEnum Type { get; set; }
    [Required]
    [StringLength(50)]
    public string Name => Type.GetDisplayName();

    public string? Description { get; set; }

    // 分类下的诗文
    public ICollection<PoemCategory> Poems { get; set; } = [];
}

// 多对多关系表， 没有额外属性，使用联合主键
public class PoemCategory
{
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}