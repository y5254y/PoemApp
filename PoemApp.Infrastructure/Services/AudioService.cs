// AudioService.cs
using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;

namespace PoemApp.Infrastructure.Services;

public class AudioService : IAudioService
{
    private readonly AppDbContext _context;

    public AudioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AudioDto>> GetAllAudiosAsync()
    {
        return await _context.Audios
            .Include(a => a.Poem)
            .Include(a => a.Uploader)
            .Include(a => a.Ratings)
            .Select(a => new AudioDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                FileUrl = a.FileUrl ?? string.Empty,
                UploaderId = a.UploaderId,
                UploaderName = a.Uploader != null ? a.Uploader.Username : "系统",
                UploadTime = a.UploadTime,
                AverageRating = a.AverageRating,
                RatingCount = a.Ratings.Count
            })
            .ToListAsync();
    }

    public async Task<AudioDetailDto> GetAudioByIdAsync(int id)
    {
        var audio = await _context.Audios
            .Include(a => a.Poem)
            .Include(a => a.Uploader)
            .Include(a => a.Ratings)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (audio == null) return null;

        // 将 AudioDetailDto 的 FileUrl 赋值改为非 null 安全方式
        return new AudioDetailDto
        {
            Id = audio.Id,
            PoemId = audio.PoemId,
            PoemTitle = audio.Poem.Title,
            FileUrl = audio.FileUrl ?? string.Empty, // 修复 CS8601
            UploaderId = audio.UploaderId,
            UploaderName = audio.Uploader != null ? audio.Uploader.Username : "系统",
            UploadTime = audio.UploadTime,
            AverageRating = audio.AverageRating,
            RatingCount = audio.Ratings.Count,
            Ratings = audio.Ratings.Select(r => new AudioRatingDto
            {
                Id = r.Id,
                AudioId = r.AudioId,
                UserId = r.UserId,
                UserName = r.User.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                RatingTime = r.RatingTime
            }).ToList()
        };
    }

    public async Task<IEnumerable<AudioDto>> GetAudiosByPoemIdAsync(int poemId)
    {
        return await _context.Audios
            .Include(a => a.Poem)
            .Include(a => a.Uploader)
            .Include(a => a.Ratings)
            .Where(a => a.PoemId == poemId)
            .Select(a => new AudioDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                FileUrl = a.FileUrl ?? string.Empty,
                UploaderId = a.UploaderId,
                UploaderName = a.Uploader != null ? a.Uploader.Username : "系统",
                UploadTime = a.UploadTime,
                AverageRating = a.AverageRating,
                RatingCount = a.Ratings.Count
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AudioDto>> GetAudiosByUploaderIdAsync(int uploaderId)
    {
        return await _context.Audios
            .Include(a => a.Poem)
            .Include(a => a.Uploader)
            .Include(a => a.Ratings)
            .Where(a => a.UploaderId == uploaderId)
            .Select(a => new AudioDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                FileUrl = a.FileUrl != null ? a.FileUrl : string.Empty,
                UploaderId = a.UploaderId,
                UploaderName = a.Uploader != null ? a.Uploader.Username : "系统",
                UploadTime = a.UploadTime,
                AverageRating = a.AverageRating,
                RatingCount = a.Ratings.Count
            })
            .ToListAsync();
    }

    public async Task<AudioDto> AddAudioAsync(CreateAudioDto audioDto)
    {
        // 检查诗文是否存在
        var poem = await _context.Poems.FindAsync(audioDto.PoemId);
        if (poem == null)
        {
            throw new ArgumentException("Poem not found");
        }

        // 如果指定了上传者，检查用户是否存在
        if (audioDto.UploaderId.HasValue)
        {
            var uploader = await _context.Users.FindAsync(audioDto.UploaderId.Value);
            if (uploader == null)
            {
                throw new ArgumentException("Uploader not found");
            }
        }

        var audio = new Audio
        {
            PoemId = audioDto.PoemId,
            FileUrl = audioDto.FileUrl,
            UploaderId = audioDto.UploaderId,
            UploadTime = DateTime.UtcNow,
            AverageRating = 0
        };

        _context.Audios.Add(audio);
        await _context.SaveChangesAsync();

        return await GetAudioByIdAsync(audio.Id);
    }

    public async Task UpdateAudioAsync(int id, UpdateAudioDto audioDto)
    {
        var audio = await _context.Audios.FindAsync(id);
        if (audio == null)
        {
            throw new ArgumentException("Audio not found");
        }

        if (!string.IsNullOrEmpty(audioDto.FileUrl))
        {
            audio.FileUrl = audioDto.FileUrl;
        }

        _context.Audios.Update(audio);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAudioAsync(int id)
    {
        var audio = await _context.Audios.FindAsync(id);
        if (audio == null)
        {
            throw new ArgumentException("Audio not found");
        }

        _context.Audios.Remove(audio);
        await _context.SaveChangesAsync();
    }

    public async Task<AudioRatingDto> AddAudioRatingAsync(int audioId, CreateAudioRatingDto ratingDto, int userId)
    {
        // 检查音频是否存在
        var audio = await _context.Audios.FindAsync(audioId);
        if (audio == null)
        {
            throw new ArgumentException("Audio not found");
        }

        // 检查用户是否存在
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        // 检查用户是否已经对该音频评过分
        var existingRating = await _context.AudioRatings
            .FirstOrDefaultAsync(r => r.AudioId == audioId && r.UserId == userId);

        if (existingRating != null)
        {
            throw new ArgumentException("User has already rated this audio");
        }

        var rating = new AudioRating
        {
            AudioId = audioId,
            UserId = userId,
            Rating = ratingDto.Rating,
            Comment = ratingDto.Comment,
            RatingTime = DateTime.UtcNow
        };

        _context.AudioRatings.Add(rating);
        await _context.SaveChangesAsync();

        // 更新音频的平均评分
        await UpdateAudioAverageRating(audioId);

        // 返回创建的评分，增加空值检查
        var createdRating = await _context.AudioRatings
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == rating.Id);

        if (createdRating == null || createdRating.User == null)
        {
            throw new InvalidOperationException("Created rating or user not found after creation.");
        }

        return new AudioRatingDto
        {
            Id = createdRating.Id,
            AudioId = createdRating.AudioId,
            UserId = createdRating.UserId,
            UserName = createdRating.User.Username,
            Rating = createdRating.Rating,
            Comment = createdRating.Comment,
            RatingTime = createdRating.RatingTime
        };
    }

    public async Task UpdateAudioRatingAsync(int ratingId, UpdateAudioRatingDto ratingDto, int userId)
    {
        var rating = await _context.AudioRatings.FindAsync(ratingId);
        if (rating == null)
        {
            throw new ArgumentException("Rating not found");
        }

        // 检查用户是否有权限修改此评分
        if (rating.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own ratings");
        }

        rating.Rating = ratingDto.Rating;
        rating.Comment = ratingDto.Comment;
        rating.RatingTime = DateTime.UtcNow;

        _context.AudioRatings.Update(rating);
        await _context.SaveChangesAsync();

        // 更新音频的平均评分
        await UpdateAudioAverageRating(rating.AudioId);
    }

    public async Task DeleteAudioRatingAsync(int ratingId, int userId)
    {
        var rating = await _context.AudioRatings.FindAsync(ratingId);
        if (rating == null)
        {
            throw new ArgumentException("Rating not found");
        }

        // 检查用户是否有权限删除此评分
        if (rating.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own ratings");
        }

        var audioId = rating.AudioId;
        _context.AudioRatings.Remove(rating);
        await _context.SaveChangesAsync();

        // 更新音频的平均评分
        await UpdateAudioAverageRating(audioId);
    }

    public async Task<IEnumerable<AudioRatingDto>> GetAudioRatingsAsync(int audioId)
    {
        return await _context.AudioRatings
            .Include(r => r.User)
            .Where(r => r.AudioId == audioId)
            .Select(r => new AudioRatingDto
            {
                Id = r.Id,
                AudioId = r.AudioId,
                UserId = r.UserId,
                UserName = r.User.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                RatingTime = r.RatingTime
            })
            .ToListAsync();
    }

    public async Task<AudioRatingDto?> GetUserAudioRatingAsync(int audioId, int userId)
    {
        var rating = await _context.AudioRatings
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.AudioId == audioId && r.UserId == userId);

        if (rating == null) return null;

        return new AudioRatingDto
        {
            Id = rating.Id,
            AudioId = rating.AudioId,
            UserId = rating.UserId,
            UserName = rating.User.Username,
            Rating = rating.Rating,
            Comment = rating.Comment,
            RatingTime = rating.RatingTime
        };
    }

    private async Task UpdateAudioAverageRating(int audioId)
    {
        var ratings = await _context.AudioRatings
            .Where(r => r.AudioId == audioId)
            .ToListAsync();

        if (ratings.Any())
        {
            var averageRating = ratings.Average(r => r.Rating);
            var audio = await _context.Audios.FindAsync(audioId);
            if (audio != null)
            {
                audio.AverageRating = Math.Round(averageRating, 2);
                _context.Audios.Update(audio);
                await _context.SaveChangesAsync();
            }
        }
    }
}