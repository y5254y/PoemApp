using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class PoemDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;

    public DynastyEnum Dynasty { get; set; }
    // 添加朝代显示名称
    public string DynastyDisplayName { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Annotation { get; set; } = string.Empty;
    // 拼音字段（可选）
    public string Pinyin { get; set; } = string.Empty;
    // 作品鉴赏
    public string Appreciation { get; set; } = string.Empty;
    // category names for display
    public List<string> Categories { get; set; } = new List<string>();
    // also include category ids for admin UI mapping
    public List<int> CategoryIds { get; set; } = new List<int>();
}