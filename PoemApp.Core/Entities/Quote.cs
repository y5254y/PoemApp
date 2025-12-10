using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class Quote
{
    public int Id { get; set; }

    // 名言/名句内容
    [Required]
    [StringLength(500)]
    public string Content { get; set; } = null!;

    // 可选的作者引用（有些名句有明确作者）
    public int? AuthorId { get; set; }
    public Author? Author { get; set; }

    // 可选的诗文引用（名句可能来自某首古诗文）
    public int? PoemId { get; set; }
    public Poem? Poem { get; set; }

    // 来源说明（例如诗名、书籍或出处）
    [StringLength(200)]
    public string? Source { get; set; }

    // 译文或现代白话
    public string? Translation { get; set; }

    // 注释/解读
    public string? Annotation { get; set; }

    // 创建时间
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}