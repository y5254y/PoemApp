// UsersController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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
            
            // 安全检查：只能查看自己的收藏（或管理员可以查看所有）
            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid("您只能查看自己的收藏");
            }
            
            var favorites = await _userService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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
            
            // 安全检查：只能为自己添加收藏
            if (currentUserId != userId)
            {
                return Forbid("您只能为自己添加收藏");
            }
            
            var createdFavorite = await _userService.AddUserFavoriteAsync(userId, favoriteDto);
            return CreatedAtAction(nameof(GetUserFavorites), new { userId = userId }, createdFavorite);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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
            
            // 安全检查：只能删除自己的收藏
            if (currentUserId != userId)
            {
                return Forbid("您只能删除自己的收藏");
            }
            
            await _userService.RemoveUserFavoriteAsync(userId, poemId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/favorites/{poemId}")]
    [Authorize]
    public async Task<ActionResult<bool>> IsPoemInFavorites(int userId, int poemId)
    {
        try
        {
            var isFavorite = await _userService.IsPoemInFavoritesAsync(userId, poemId);
            return Ok(isFavorite);
        }
        catch (ArgumentException ex)
        {
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
            var points = await _userService.GetUserPointsAsync(userId);
            return Ok(points);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/points/records")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PointsRecordDto>>> GetUserPointsRecords(int userId)
    {
        try
        {
            var records = await _userService.GetUserPointsRecordsAsync(userId);
            return Ok(records);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/points")]
    [Authorize]
    public async Task<ActionResult<PointsRecordDto>> PostUserPoints(int userId, AddPointsDto pointsDto)
    {
        try
        {
            var record = await _userService.AddPointsAsync(userId, pointsDto);
            return CreatedAtAction(nameof(GetUserPointsRecords), new { userId = userId }, record);
        }
        catch (ArgumentException ex)
        {
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
            await _userService.UpdateVipStatusAsync(userId, vipDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/vip")]
    [Authorize]
    public async Task<IActionResult> DeleteUserVip(int userId)
    {
        try
        {
            await _userService.RemoveVipStatusAsync(userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
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
            var annotations = await _userService.GetUserAnnotationsAsync(userId);
            return Ok(annotations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/audios")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AudioDto>>> GetUserAudios(int userId)
    {
        try
        {
            var audios = await _userService.GetUserUploadedAudiosAsync(userId);
            return Ok(audios);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/change-password")]
    [Authorize]
    public async Task<IActionResult> ChangeUserPassword(int userId, ChangePasswordDto dto)
    {
        try
        {
            await _userService.UpdateUserPasswordAsync(userId, dto.NewPassword);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
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
            var favorites = await _userService.GetUserQuoteFavoritesAsync(userId);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/quote-favorites")]
    [Authorize]
    public async Task<ActionResult<UserQuoteFavoriteDto>> PostUserQuoteFavorite(int userId, CreateUserQuoteFavoriteDto favoriteDto)
    {
        try
        {
            var created = await _userService.AddUserQuoteFavoriteAsync(userId, favoriteDto);
            return CreatedAtAction(nameof(GetUserQuoteFavorites), new { userId = userId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/quote-favorites/{quoteId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserQuoteFavorite(int userId, int quoteId)
    {
        try
        {
            await _userService.RemoveUserQuoteFavoriteAsync(userId, quoteId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/quote-favorites/{quoteId}")]
    [Authorize]
    public async Task<ActionResult<bool>> IsQuoteInFavorites(int userId, int quoteId)
    {
        try
        {
            var isFav = await _userService.IsQuoteInFavoritesAsync(userId, quoteId);
            return Ok(isFav);
        }
        catch (ArgumentException ex)
        {
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