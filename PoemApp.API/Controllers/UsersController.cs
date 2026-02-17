// UsersController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<PagedResult<BasicUserDto>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] UserRole? role = null, [FromQuery] bool? isVip = null)
    {
        var result = await _userService.GetUsersAsync(page, pageSize, search, role, isVip);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DetailedUserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicUserDto>> PostUser(CreateUserDto userDto)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicUserDto>> PutUser(int id, UpdateUserDto userDto)
    {
        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userDto);
            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 用户收藏管理
    [HttpGet("{userId}/favorites")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserFavoriteDto>>> GetUserFavorites(int userId)
    {
        try
        {
            // 从 Token 中获取当前登录用户的 ID
            var currentUserId = GetCurrentUserIdFromToken();
            
            _logger.LogInformation("获取收藏列表 - 当前登录用户ID: {CurrentUserId}, 请求查询用户ID: {RequestedUserId}", currentUserId, userId);
            
            // 安全检查：只能查看自己的收藏（或管理员可以查看所有）
            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("权限不足 - 用户 {CurrentUserId} 尝试查看用户 {RequestedUserId} 的收藏", currentUserId, userId);
                return StatusCode(403, new { message = "您只能查看自己的收藏" });
            }
            
            var favorites = await _userService.GetUserFavoritesAsync(userId);
            _logger.LogInformation("成功获取收藏列表 - 用户ID: {UserId}, 收藏数量: {Count}", userId, favorites?.Count() ?? 0);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取收藏列表失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取收藏列表时发生未处理异常 - 用户ID: {UserId}", userId);
            throw;
        }
    }

    [HttpPost("{userId}/favorites")]
    [Authorize]
    public async Task<ActionResult<UserFavoriteDto>> PostUserFavorite(int userId, CreateUserFavoriteDto favoriteDto)
    {
        try
        {
            // 从 Token 中获取当前登录用户的 ID
            var currentUserId = GetCurrentUserIdFromToken();
            
            _logger.LogInformation("添加收藏 - 当前登录用户ID: {CurrentUserId}, 请求操作用户ID: {RequestedUserId}, 诗文ID: {PoemId}", 
                currentUserId, userId, favoriteDto.PoemId);
            
            // 安全检查：只能为自己添加收藏
            if (currentUserId != userId)
            {
                _logger.LogWarning("权限不足 - 用户 {CurrentUserId} 尝试为用户 {RequestedUserId} 添加收藏", currentUserId, userId);
                return StatusCode(403, new { message = "您只能为自己添加收藏" });
            }
            
            var createdFavorite = await _userService.AddUserFavoriteAsync(userId, favoriteDto);
            _logger.LogInformation("成功添加收藏 - 用户ID: {UserId}, 诗文ID: {PoemId}", userId, favoriteDto.PoemId);
            return CreatedAtAction(nameof(GetUserFavorites), new { userId = userId }, createdFavorite);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("添加收藏失败 - 用户ID: {UserId}, 诗文ID: {PoemId}, 错误: {Error}", 
                userId, favoriteDto.PoemId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加收藏时发生未处理异常 - 用户ID: {UserId}, 诗文ID: {PoemId}", 
                userId, favoriteDto.PoemId);
            throw;
        }
    }

    [HttpDelete("{userId}/favorites/{poemId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserFavorite(int userId, int poemId)
    {
        try
        {
            // 从 Token 中获取当前登录用户的 ID
            var currentUserId = GetCurrentUserIdFromToken();
            
            _logger.LogInformation("删除收藏 - 当前登录用户ID: {CurrentUserId}, 请求操作用户ID: {RequestedUserId}, 诗文ID: {PoemId}", 
                currentUserId, userId, poemId);
            
            // 安全检查：只能删除自己的收藏
            if (currentUserId != userId)
            {
                _logger.LogWarning("权限不足 - 用户 {CurrentUserId} 尝试删除用户 {RequestedUserId} 的收藏", currentUserId, userId);
                return StatusCode(403, new { message = "您只能删除自己的收藏" });
            }
            
            await _userService.RemoveUserFavoriteAsync(userId, poemId);
            _logger.LogInformation("成功删除收藏 - 用户ID: {UserId}, 诗文ID: {PoemId}", userId, poemId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("删除收藏失败 - 用户ID: {UserId}, 诗文ID: {PoemId}, 错误: {Error}", 
                userId, poemId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除收藏时发生未处理异常 - 用户ID: {UserId}, 诗文ID: {PoemId}", 
                userId, poemId);
            throw;
        }
    }

    [HttpGet("{userId}/favorites/{poemId}")]
    [Authorize]
    public async Task<ActionResult<bool>> IsPoemInFavorites(int userId, int poemId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("检查收藏状态 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}, 诗文ID: {PoemId}", 
                currentUserId, userId, poemId);
            
            var isFavorite = await _userService.IsPoemInFavoritesAsync(userId, poemId);
            _logger.LogInformation("收藏状态查询结果 - 用户ID: {UserId}, 诗文ID: {PoemId}, 是否收藏: {IsFavorite}", 
                userId, poemId, isFavorite);
            return Ok(isFavorite);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("检查收藏状态失败 - 用户ID: {UserId}, 诗文ID: {PoemId}, 错误: {Error}", 
                userId, poemId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    // 积分管理
    [HttpGet("{userId}/points")]
    [Authorize]
    public async Task<ActionResult<int>> GetUserPoints(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("获取积分 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            var points = await _userService.GetUserPointsAsync(userId);
            _logger.LogInformation("成功获取积分 - 用户ID: {UserId}, 积分: {Points}", userId, points);
            return Ok(points);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取积分失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/points/records")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PointsRecordDto>>> GetUserPointsRecords(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("获取积分记录 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            var records = await _userService.GetUserPointsRecordsAsync(userId);
            _logger.LogInformation("成功获取积分记录 - 用户ID: {UserId}, 记录数量: {Count}", 
                userId, records?.Count() ?? 0);
            return Ok(records);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取积分记录失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/points")]
    [Authorize]
    public async Task<ActionResult<PointsRecordDto>> PostUserPoints(int userId, AddPointsDto pointsDto)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("添加积分 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}, 积分: {Points}, 来源: {Source}", 
                currentUserId, userId, pointsDto.Points, pointsDto.Source);
            
            var record = await _userService.AddPointsAsync(userId, pointsDto);
            _logger.LogInformation("成功添加积分 - 用户ID: {UserId}, 积分: {Points}", userId, pointsDto.Points);
            return CreatedAtAction(nameof(GetUserPointsRecords), new { userId = userId }, record);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("添加积分失败 - 用户ID: {UserId}, 积分: {Points}, 错误: {Error}", 
                userId, pointsDto.Points, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    // VIP管理
    [HttpPut("{userId}/vip")]
    [Authorize]
    public async Task<IActionResult> PutUserVip(int userId, UpdateVipStatusDto vipDto)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("更新VIP状态 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}, VIP结束时间: {VipEndDate}", 
                currentUserId, userId, vipDto.VipEndDate);
            
            await _userService.UpdateVipStatusAsync(userId, vipDto);
            _logger.LogInformation("成功更新VIP状态 - 用户ID: {UserId}", userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("更新VIP状态失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/vip")]
    [Authorize]
    public async Task<IActionResult> DeleteUserVip(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("删除VIP状态 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            await _userService.RemoveVipStatusAsync(userId);
            _logger.LogInformation("成功删除VIP状态 - 用户ID: {UserId}", userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("删除VIP状态失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    // 用户内容管理
    [HttpGet("{userId}/annotations")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AnnotationDto>>> GetUserAnnotations(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("获取标注列表 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            var annotations = await _userService.GetUserAnnotationsAsync(userId);
            _logger.LogInformation("成功获取标注列表 - 用户ID: {UserId}, 标注数量: {Count}", 
                userId, annotations?.Count() ?? 0);
            return Ok(annotations);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取标注列表失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/audios")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AudioDto>>> GetUserAudios(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("获取音频列表 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            var audios = await _userService.GetUserUploadedAudiosAsync(userId);
            _logger.LogInformation("成功获取音频列表 - 用户ID: {UserId}, 音频数量: {Count}", 
                userId, audios?.Count() ?? 0);
            return Ok(audios);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取音频列表失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/change-password")]
    [Authorize]
    public async Task<IActionResult> ChangeUserPassword(int userId, ChangePasswordDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("修改密码 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            await _userService.UpdateUserPasswordAsync(userId, dto.NewPassword);
            _logger.LogInformation("成功修改密码 - 用户ID: {UserId}", userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("修改密码失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    // 用户对名句的收藏管理
    [HttpGet("{userId}/quote-favorites")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserQuoteFavoriteDto>>> GetUserQuoteFavorites(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("获取名句收藏列表 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}", 
                currentUserId, userId);
            
            var favorites = await _userService.GetUserQuoteFavoritesAsync(userId);
            _logger.LogInformation("成功获取名句收藏列表 - 用户ID: {UserId}, 收藏数量: {Count}", 
                userId, favorites?.Count() ?? 0);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("获取名句收藏列表失败 - 用户ID: {UserId}, 错误: {Error}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/quote-favorites")]
    [Authorize]
    public async Task<ActionResult<UserQuoteFavoriteDto>> PostUserQuoteFavorite(int userId, CreateUserQuoteFavoriteDto favoriteDto)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("添加名句收藏 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}, 名句ID: {QuoteId}", 
                currentUserId, userId, favoriteDto.QuoteId);
            
            var created = await _userService.AddUserQuoteFavoriteAsync(userId, favoriteDto);
            _logger.LogInformation("成功添加名句收藏 - 用户ID: {UserId}, 名句ID: {QuoteId}", userId, favoriteDto.QuoteId);
            return CreatedAtAction(nameof(GetUserQuoteFavorites), new { userId = userId }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("添加名句收藏失败 - 用户ID: {UserId}, 名句ID: {QuoteId}, 错误: {Error}", 
                userId, favoriteDto.QuoteId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/quote-favorites/{quoteId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserQuoteFavorite(int userId, int quoteId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("删除名句收藏 - 当前登录用户ID: {CurrentUserId}, 操作用户ID: {RequestedUserId}, 名句ID: {QuoteId}", 
                currentUserId, userId, quoteId);
            
            await _userService.RemoveUserQuoteFavoriteAsync(userId, quoteId);
            _logger.LogInformation("成功删除名句收藏 - 用户ID: {UserId}, 名句ID: {QuoteId}", userId, quoteId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("删除名句收藏失败 - 用户ID: {UserId}, 名句ID: {QuoteId}, 错误: {Error}", 
                userId, quoteId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除名句收藏时发生未处理异常 - 用户ID: {UserId}, 名句ID: {QuoteId}", 
                userId, quoteId);
            throw;
        }
    }

    [HttpGet("{userId}/quote-favorites/{quoteId}")]
    [Authorize]
    public async Task<ActionResult<bool>> IsQuoteInFavorites(int userId, int quoteId)
    {
        try
        {
            var currentUserId = GetCurrentUserIdFromToken();
            _logger.LogInformation("检查名句收藏状态 - 当前登录用户ID: {CurrentUserId}, 查询用户ID: {RequestedUserId}, 名句ID: {QuoteId}", 
                currentUserId, userId, quoteId);
            
            var isFav = await _userService.IsQuoteInFavoritesAsync(userId, quoteId);
            _logger.LogInformation("名句收藏状态查询结果 - 用户ID: {UserId}, 名句ID: {QuoteId}, 是否收藏: {IsFavorite}", 
                userId, quoteId, isFav);
            return Ok(isFav);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("检查名句收藏状态失败 - 用户ID: {UserId}, 名句ID: {QuoteId}, 错误: {Error}", 
                userId, quoteId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<DetailedUserDto>> GetUserDetails(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // 添加辅助方法：从 Token 中获取当前用户 ID
    private int GetCurrentUserIdFromToken()
    {
        var claimNames = new[] 
        { 
            System.Security.Claims.ClaimTypes.NameIdentifier,
            "nameid",
            "sub",
            "userId",
            "id"
        };

        foreach (var claimName in claimNames)
        {
            var claimValue = User.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out var userId) && userId > 0)
            {
                return userId;
            }
        }

        throw new UnauthorizedAccessException("无法从 Token 中获取用户 ID");
    }
}