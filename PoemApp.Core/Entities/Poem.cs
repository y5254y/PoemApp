using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoemApp.Core.Entities;

public class Poem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public string? Background { get; set; } // 写作背景

    public string? Translation { get; set; } // 译文

    public string? Annotation { get; set; } // 注释

    // 作品鉴赏
    public string? Appreciation { get; set; }

    // 分类目录
    public ICollection<PoemCategory> Categories { get; set; } = [];

    // 音频集合
    public ICollection<Audio> Audios { get; set; } = [];

    // 用户标注
    public ICollection<Annotation> Annotations { get; set; } = [];

    // 收藏用户
    public ICollection<UserFavorite> FavoritedBy { get; set; } = [];
}