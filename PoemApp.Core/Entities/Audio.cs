

using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.Entities;

public class Audio
{
    public int Id { get; set; }

    [Required]
    public int PoemId { get; set; }
    public Poem Poem { get; set; } = null!;

    [StringLength(255)]
    public string? FileUrl { get; set; }

    public int? UploaderId { get; set; } // 可为空（系统上传）
    public User Uploader { get; set; } = null!;

    public DateTime UploadTime { get; set; } = DateTime.UtcNow;

    public double AverageRating { get; set; } = 0;

    // 评分记录
    public ICollection<AudioRating> Ratings { get; set; } = [];
}

public class AudioRating
{
    public int Id { get; set; }

    [Required]
    public int AudioId { get; set; }
    public Audio Audio { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public DateTime RatingTime { get; set; } = DateTime.UtcNow;

    public string? Comment { get; set; } // 可选的评论字段
}