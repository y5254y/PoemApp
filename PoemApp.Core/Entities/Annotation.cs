using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;
public class Annotation
{
    public int Id { get; set; }

    [Required]
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [StringLength(400)]
    public string HighlightText { get; set; } = null!; // 标注的文本

    [Required]
    [StringLength(600)]
    public string Comment { get; set; } = null!;

    public int StartIndex { get; set; } // 文本起始位置
    public int EndIndex { get; set; }   // 文本结束位置

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}