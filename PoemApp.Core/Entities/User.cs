using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class User
{
    public int Id { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    // 密码相关字段（如果需要用户名密码登录）
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    [StringLength(50)]
    public string? WeChatId { get; set; }

    [StringLength(50)]
    public string? QQId { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    // 新增字段：用户角色/权限
    public UserRole Role { get; set; } = UserRole.Normal;

    // 新增字段：VIP开始时间
    public DateTime? VipStartDate { get; set; }

    // 新增字段：VIP结束时间
    public DateTime? VipEndDate { get; set; }

    // 计算属性：判断用户当前是否是VIP
    public bool IsVip => VipEndDate.HasValue && VipEndDate.Value > DateTime.Now;

    // 新增积分字段
    public int Points { get; set; } = 0;

    // 积分获取记录
    public ICollection<PointsRecord> PointsRecords { get; set; } = [];

    // 收藏的诗文
    public ICollection<UserFavorite> Favorites { get; set; } = [];

    // 收藏的名句
    public ICollection<UserQuoteFavorite> QuoteFavorites { get; set; } = [];

    // 上传的音频
    public ICollection<Audio> Audios { get; set; } = [];

    // 评分记录
    public ICollection<AudioRating> Ratings { get; set; } = [];

    // 标注记录
    public ICollection<Annotation> Annotations { get; set; } = [];

    // 背诵记录
    public ICollection<UserRecitation> Recitations { get; set; } = [];

    // 获得的成就
    public ICollection<UserAchievement> Achievements { get; set; } = [];
}


// 积分获取记录实体
public class PointsRecord
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public PointsSource Source { get; set; }

    [Required]
    public int Points { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 关联的业务ID（如签到记录ID、任务完成记录ID等）
    public int? RelatedId { get; set; }
}

public class UserFavorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    public DateTime FavoriteTime { get; set; } = DateTime.UtcNow;
}

// 新增：用户对名句的收藏实体
public class UserQuoteFavorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int QuoteId { get; set; }
    public Quote Quote { get; set; } = null!;

    public DateTime FavoriteTime { get; set; } = DateTime.UtcNow;
}