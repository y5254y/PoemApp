// UserService.cs
using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions;

namespace PoemApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                WeChatId = u.WeChatId,
                QQId = u.QQId,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                Role = u.Role,
                RoleDisplayName = u.Role.GetDisplayName(),
                VipStartDate = u.VipStartDate,
                VipEndDate = u.VipEndDate,
                IsVip = u.IsVip,
                Points = u.Points
            })
            .ToListAsync();
    }

    public async Task<UserDetailDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Favorites)
                .ThenInclude(uf => uf.Poem)
                    .ThenInclude(p => p.Author)
            .Include(u => u.PointsRecords)
            .Include(u => u.Audios)
                .ThenInclude(a => a.Poem)
            .Include(u => u.Annotations)
                .ThenInclude(a => a.Poem)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new UserDetailDto
        {
            Id = user.Id,
            Username = user.Username,
            WeChatId = user.WeChatId,
            QQId = user.QQId,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            Role = user.Role,
            RoleDisplayName = user.Role.GetDisplayName(),
            VipStartDate = user.VipStartDate,
            VipEndDate = user.VipEndDate,
            IsVip = user.IsVip,
            Points = user.Points,
            Favorites = user.Favorites.Select(uf => new UserFavoriteDto
            {
                UserId = uf.UserId,
                PoemId = uf.PoemId,
                PoemTitle = uf.Poem.Title,
                AuthorName = uf.Poem.Author.Name,
                FavoriteTime = uf.FavoriteTime
            }).ToList(),
            PointsRecords = user.PointsRecords.Select(pr => new PointsRecordDto
            {
                Id = pr.Id,
                UserId = pr.UserId,
                Source = pr.Source,
                SourceDisplayName = pr.Source.GetDisplayName(),
                Points = pr.Points,
                Description = pr.Description,
                CreatedAt = pr.CreatedAt,
                RelatedId = pr.RelatedId
            }).ToList(),
            UploadedAudios = user.Audios.Select(a => new AudioDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                FileUrl = a.FileUrl,
                UploaderId = a.UploaderId,
                UploadTime = a.UploadTime,
                AverageRating = a.AverageRating,
                RatingCount = a.Ratings.Count
            }).ToList(),
            Annotations = user.Annotations.Select(a => new AnnotationDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                UserId = a.UserId,
                UserName = user.Username,
                HighlightText = a.HighlightText,
                Comment = a.Comment,
                StartIndex = a.StartIndex,
                EndIndex = a.EndIndex,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
    {
        // 检查用户名是否已存在
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == userDto.Username);
        if (existingUser != null)
        {
            throw new ArgumentException("Username already exists");
        }

        var user = new User
        {
            Username = userDto.Username,
            WeChatId = userDto.WeChatId,
            QQId = userDto.QQId,
            Phone = userDto.Phone,
            Role = userDto.Role,
            CreatedAt = DateTime.UtcNow,
            Points = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GetUserByIdAsync(user.Id);
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        // 如果提供了新用户名，检查是否与其他用户冲突
        if (!string.IsNullOrEmpty(userDto.Username) && userDto.Username != user.Username)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userDto.Username && u.Id != id);
            if (existingUser != null)
            {
                throw new ArgumentException("Username already exists");
            }
            user.Username = userDto.Username;
        }

        if (userDto.WeChatId != null) user.WeChatId = userDto.WeChatId;
        if (userDto.QQId != null) user.QQId = userDto.QQId;
        if (userDto.Phone != null) user.Phone = userDto.Phone;
        if (userDto.Role.HasValue) user.Role = userDto.Role.Value;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return await GetUserByIdAsync(user.Id);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserFavoriteDto>> GetUserFavoritesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Favorites)
                .ThenInclude(uf => uf.Poem)
                    .ThenInclude(p => p.Author)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new ArgumentException("User not found");

        return user.Favorites.Select(uf => new UserFavoriteDto
        {
            UserId = uf.UserId,
            PoemId = uf.PoemId,
            PoemTitle = uf.Poem.Title,
            AuthorName = uf.Poem.Author.Name,
            FavoriteTime = uf.FavoriteTime
        }).ToList();
    }

    public async Task<UserFavoriteDto> AddUserFavoriteAsync(int userId, CreateUserFavoriteDto favoriteDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        var poem = await _context.Poems.FindAsync(favoriteDto.PoemId);
        if (poem == null) throw new ArgumentException("Poem not found");

        // 检查是否已经收藏
        var existingFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.PoemId == favoriteDto.PoemId);

        if (existingFavorite != null)
        {
            throw new ArgumentException("Poem is already in favorites");
        }

        var userFavorite = new UserFavorite
        {
            UserId = userId,
            PoemId = favoriteDto.PoemId,
            FavoriteTime = DateTime.UtcNow
        };

        _context.UserFavorites.Add(userFavorite);
        await _context.SaveChangesAsync();

        // 返回创建的收藏
        var createdFavorite = await _context.UserFavorites
            .Include(uf => uf.Poem)
                .ThenInclude(p => p.Author)
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.PoemId == favoriteDto.PoemId);

        return new UserFavoriteDto
        {
            UserId = createdFavorite.UserId,
            PoemId = createdFavorite.PoemId,
            PoemTitle = createdFavorite.Poem.Title,
            AuthorName = createdFavorite.Poem.Author.Name,
            FavoriteTime = createdFavorite.FavoriteTime
        };
    }

    public async Task RemoveUserFavoriteAsync(int userId, int poemId)
    {
        var userFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.PoemId == poemId);

        if (userFavorite == null)
        {
            throw new ArgumentException("Favorite not found");
        }

        _context.UserFavorites.Remove(userFavorite);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsPoemInFavoritesAsync(int userId, int poemId)
    {
        return await _context.UserFavorites
            .AnyAsync(uf => uf.UserId == userId && uf.PoemId == poemId);
    }

    public async Task<IEnumerable<PointsRecordDto>> GetUserPointsRecordsAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.PointsRecords)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new ArgumentException("User not found");

        return user.PointsRecords.Select(pr => new PointsRecordDto
        {
            Id = pr.Id,
            UserId = pr.UserId,
            Source = pr.Source,
            SourceDisplayName = pr.Source.GetDisplayName(),
            Points = pr.Points,
            Description = pr.Description,
            CreatedAt = pr.CreatedAt,
            RelatedId = pr.RelatedId
        }).ToList();
    }

    public async Task<int> GetUserPointsAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        return user.Points;
    }

    public async Task<PointsRecordDto> AddPointsAsync(int userId, AddPointsDto pointsDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        var pointsRecord = new PointsRecord
        {
            UserId = userId,
            Source = pointsDto.Source,
            Points = pointsDto.Points,
            Description = pointsDto.Description,
            RelatedId = pointsDto.RelatedId,
            CreatedAt = DateTime.UtcNow
        };

        // 更新用户总积分
        user.Points += pointsDto.Points;

        _context.PointsRecords.Add(pointsRecord);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new PointsRecordDto
        {
            Id = pointsRecord.Id,
            UserId = pointsRecord.UserId,
            Source = pointsRecord.Source,
            SourceDisplayName = pointsRecord.Source.GetDisplayName(),
            Points = pointsRecord.Points,
            Description = pointsRecord.Description,
            CreatedAt = pointsRecord.CreatedAt,
            RelatedId = pointsRecord.RelatedId
        };
    }

    public async Task UpdateVipStatusAsync(int userId, UpdateVipStatusDto vipDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        user.VipStartDate = vipDto.VipStartDate;
        user.VipEndDate = vipDto.VipEndDate;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveVipStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        user.VipStartDate = null;
        user.VipEndDate = null;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AnnotationDto>> GetUserAnnotationsAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Annotations)
                .ThenInclude(a => a.Poem)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new ArgumentException("User not found");

        return user.Annotations.Select(a => new AnnotationDto
        {
            Id = a.Id,
            PoemId = a.PoemId,
            PoemTitle = a.Poem.Title,
            UserId = a.UserId,
            UserName = user.Username,
            HighlightText = a.HighlightText,
            Comment = a.Comment,
            StartIndex = a.StartIndex,
            EndIndex = a.EndIndex,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<IEnumerable<AudioDto>> GetUserUploadedAudiosAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Audios)
                .ThenInclude(a => a.Poem)
            .Include(u => u.Audios)
                .ThenInclude(a => a.Ratings)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new ArgumentException("User not found");

        return user.Audios.Select(a => new AudioDto
        {
            Id = a.Id,
            PoemId = a.PoemId,
            PoemTitle = a.Poem.Title,
            FileUrl = a.FileUrl,
            UploaderId = a.UploaderId,
            UploadTime = a.UploadTime,
            AverageRating = a.AverageRating,
            RatingCount = a.Ratings.Count
        }).ToList();
    }
}