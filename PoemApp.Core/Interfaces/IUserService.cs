// IUserService.cs
using PoemApp.Core.DTOs;
using PoemApp.Core.Enums;

namespace PoemApp.Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null, UserRole? role = null, bool? isVip = null);
    Task<UserDetailDto> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserDto userDto);
    Task<UserDto> UpdateUserAsync(int id, UpdateUserDto userDto);
    Task DeleteUserAsync(int id);
    Task UpdateUserPasswordAsync(int userId, string newPassword);

    // 用户收藏管理
    Task<IEnumerable<UserFavoriteDto>> GetUserFavoritesAsync(int userId);
    Task<UserFavoriteDto> AddUserFavoriteAsync(int userId, CreateUserFavoriteDto favoriteDto);
    Task RemoveUserFavoriteAsync(int userId, int poemId);
    Task<bool> IsPoemInFavoritesAsync(int userId, int poemId);

    // 用户对名句收藏管理
    Task<IEnumerable<UserQuoteFavoriteDto>> GetUserQuoteFavoritesAsync(int userId);
    Task<UserQuoteFavoriteDto> AddUserQuoteFavoriteAsync(int userId, CreateUserQuoteFavoriteDto favoriteDto);
    Task RemoveUserQuoteFavoriteAsync(int userId, int quoteId);
    Task<bool> IsQuoteInFavoritesAsync(int userId, int quoteId);

    // 积分管理
    Task<IEnumerable<PointsRecordDto>> GetUserPointsRecordsAsync(int userId);
    Task<int> GetUserPointsAsync(int userId);
    Task<PointsRecordDto> AddPointsAsync(int userId, AddPointsDto pointsDto);

    // VIP管理
    Task UpdateVipStatusAsync(int userId, UpdateVipStatusDto vipDto);
    Task RemoveVipStatusAsync(int userId);

    // 用户内容管理
    Task<IEnumerable<AnnotationDto>> GetUserAnnotationsAsync(int userId);
    Task<IEnumerable<AudioDto>> GetUserUploadedAudiosAsync(int userId);
}