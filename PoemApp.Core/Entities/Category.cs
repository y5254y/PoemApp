
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // 分类下的诗文
    public ICollection<PoemCategory> Poems { get; set; } = [];
}

// 多对多关系表
public class PoemCategory
{
    public int Id { get; set; }
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}