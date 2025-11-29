using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PoemApp.Core.Enums;

namespace PoemApp.Core.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty; // 可自由输入，如 "小学一年级人教版"

    public string? Description { get; set; }

    // 可选：用于标识分类所属的“维度”或“组”，便于前端分组展示或过滤
    public CategoryGroup? Group { get; set; } // 如 "EducationLevel", "LiteraryForm", "Theme" 等

    // 
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    // 关联诗文（多对多）
    public ICollection<PoemCategory> Poems { get; set; } = new List<PoemCategory>();
}

// 多对多关系表， 没有额外属性，使用联合主键
public class PoemCategory
{
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}