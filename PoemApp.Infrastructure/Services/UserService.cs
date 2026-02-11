// UserService.cs
using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions;
using PoemApp.Core.Enums;
using System.Security.Cryptography;
using System.Text;
using PoemApp.Core.DTOs;

namespace PoemApp.Infrastructure.Services;

public partial class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BasicUserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new BasicUserDto
            {
                Id = u.Id,
                Username = u.Username,
                WeChatId = u.WeChatId,
                QQId = u.QQId,
                Phone = u.Phone,
                Points = u.Points,
                Role = u.Role.ToString(),
                RoleDisplayName = u.Role.GetDisplayName()
            })
            .ToListAsync();
    }

    public async Task<DetailedUserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Favorites).ThenInclude(f => f.Poem).ThenInclude(p => p.Author)
            .Include(u => u.Recitations).ThenInclude(r => r.Poem)
            .Include(u => u.Achievements).ThenInclude(a => a.Achievement)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) throw new Exception("User not found");

        return new DetailedUserDto
        {
            Id = user.Id,
            Username = user.Username,
            WeChatId = user.WeChatId,
            QQId = user.QQId,
            Phone = user.Phone,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            VipStartDate = user.VipStartDate,
            VipEndDate = user.VipEndDate,
            Points = user.Points,
            Favorites = user.Favorites.Select(f => new PoemDto
            {
                Id = f.PoemId,
                Title = f.Poem.Title,
                AuthorName = f.Poem.Author.Name
            }).ToList(),
            Recitations = user.Recitations.Select(r => new RecitationDto
            {
                Id = r.Id,
                PoemId = r.PoemId,
                PoemTitle = r.Poem.Title,
                Status = r.Status.ToString(),
                NextReviewTime = r.NextReviewTime,
                Proficiency = r.Proficiency
            }).ToList(),
            Achievements = user.Achievements.Select(a => new UserAchievementDto
            {
                Id = a.Id,
                AchievementName = a.Achievement.Name,
                AchievedAt = a.AchievedAt,
                CurrentValue = a.CurrentValue,
                RewardClaimed = a.RewardClaimed
            }).ToList()
        };
    }

    public async Task<BasicUserDto> CreateUserAsync(CreateUserDto userDto)
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
            CreatedAt = DateTime.UtcNow
        };

        // Ensure password hash/salt are set because database columns are non-nullable.
        // If caller didn't provide a password (e.g. third-party login), generate a random fallback password and store its hash.
        CreatePasswordHash(string.IsNullOrEmpty(userDto.Password) ? Guid.NewGuid().ToString() : userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new BasicUserDto
        {
            Id = user.Id,
            Username = user.Username,
            WeChatId = user.WeChatId,
            QQId = user.QQId,
            Phone = user.Phone,
            Points = user.Points,
            Role = user.Role.ToString(),
            RoleDisplayName = user.Role.GetDisplayName()
        };
    }

    public async Task<BasicUserDto> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.Username = userDto.Username;
        user.Phone = userDto.Phone;
        if (userDto.Role.HasValue)
        {
            user.Role = userDto.Role.Value;
        }
        user.WeChatId = userDto.WeChatId;
        user.QQId = userDto.QQId;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new BasicUserDto
        {
            Id = user.Id,
            Username = user.Username,
            WeChatId = user.WeChatId,
            QQId = user.QQId,
            Phone = user.Phone,
            Points = user.Points,
            Role = user.Role.ToString(),
            RoleDisplayName = user.Role.GetDisplayName()
        };
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

        if (createdFavorite == null || createdFavorite.Poem == null || createdFavorite.Poem.Author == null)
        {
            throw new InvalidOperationException("Failed to retrieve created favorite with related data.");
        }

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
            FileUrl = a.FileUrl ?? string.Empty, // 修复 CS8601
            UploaderId = a.UploaderId,
            UploadTime = a.UploadTime,
            AverageRating = a.AverageRating,
            RatingCount = a.Ratings.Count
        }).ToList();
    }

    public async Task<PagedResult<BasicUserDto>> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null, UserRole? role = null, bool? isVip = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Username.Contains(search));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (isVip.HasValue)
        {
            query = query.Where(u => u.IsVip == isVip.Value);
        }

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new BasicUserDto
            {
                Id = u.Id,
                Username = u.Username,
                WeChatId = u.WeChatId,
                QQId = u.QQId,
                Phone = u.Phone,
                Points = u.Points,
                Role = u.Role.ToString(),
                RoleDisplayName = u.Role.GetDisplayName()
            })
            .ToListAsync();

        return new PagedResult<BasicUserDto>
        {
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task UpdateUserPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters");

        CreatePasswordHash(newPassword, out var hash, out var salt);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserQuoteFavoriteDto>> GetUserQuoteFavoritesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.QuoteFavorites)
                .ThenInclude(qf => qf.Quote)
                    .ThenInclude(q => q.Author)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new ArgumentException("User not found");

        return user.QuoteFavorites.Select(qf => new UserQuoteFavoriteDto
        {
            UserId = qf.UserId,
            QuoteId = qf.QuoteId,
            QuoteContent = qf.Quote.Content,
            AuthorName = qf.Quote.Author?.Name,
            FavoriteTime = qf.FavoriteTime
        }).ToList();
    }

    public async Task<UserQuoteFavoriteDto> AddUserQuoteFavoriteAsync(int userId, CreateUserQuoteFavoriteDto favoriteDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        var quote = await _context.Quotes.FindAsync(favoriteDto.QuoteId);
        if (quote == null) throw new ArgumentException("Quote not found");

        var existing = await _context.UserQuoteFavorites
            .FirstOrDefaultAsync(qf => qf.UserId == userId && qf.QuoteId == favoriteDto.QuoteId);
        if (existing != null) throw new ArgumentException("Quote is already in favorites");

        var userQuoteFavorite = new UserQuoteFavorite
        {
            UserId = userId,
            QuoteId = favoriteDto.QuoteId,
            FavoriteTime = DateTime.UtcNow
        };

        _context.UserQuoteFavorites.Add(userQuoteFavorite);
        await _context.SaveChangesAsync();

        var created = await _context.UserQuoteFavorites
            .Include(qf => qf.Quote)
                .ThenInclude(q => q.Author)
            .FirstOrDefaultAsync(qf => qf.UserId == userId && qf.QuoteId == favoriteDto.QuoteId);

        if (created == null || created.Quote == null) throw new InvalidOperationException("Failed to retrieve created quote favorite");

        return new UserQuoteFavoriteDto
        {
            UserId = created.UserId,
            QuoteId = created.QuoteId,
            QuoteContent = created.Quote.Content,
            AuthorName = created.Quote.Author?.Name,
            FavoriteTime = created.FavoriteTime
        };
    }

    public async Task RemoveUserQuoteFavoriteAsync(int userId, int quoteId)
    {
        var qf = await _context.UserQuoteFavorites
            .FirstOrDefaultAsync(x => x.UserId == userId && x.QuoteId == quoteId);
        if (qf == null) throw new ArgumentException("Favorite not found");

        _context.UserQuoteFavorites.Remove(qf);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsQuoteInFavoritesAsync(int userId, int quoteId)
    {
        return await _context.UserQuoteFavorites
            .AnyAsync(qf => qf.UserId == userId && qf.QuoteId == quoteId);
    }

    // Helper
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}