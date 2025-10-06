
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class User
{
    public int Id { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(50)]
    public string? WeChatId { get; set; }

    [StringLength(50)]
    public string? QQId { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 收藏的诗文
    public ICollection<UserFavorite> Favorites { get; set; } = [];

    // 上传的音频
    public ICollection<Audio> Audios { get; set; } = [];

    // 评分记录
    public ICollection<AudioRating> Ratings { get; set; } = [];

    // 标注记录
    public ICollection<Annotation> Annotations { get; set; } = [];
}

public class UserFavorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    public DateTime FavoriteTime { get; set; } = DateTime.UtcNow;
}