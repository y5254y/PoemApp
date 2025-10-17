// UsersController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> PostUser(CreateUserDto userDto)
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
    public async Task<ActionResult<UserDto>> PutUser(int id, UpdateUserDto userDto)
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
    public async Task<ActionResult<IEnumerable<UserFavoriteDto>>> GetUserFavorites(int userId)
    {
        try
        {
            var favorites = await _userService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/favorites")]
    public async Task<ActionResult<UserFavoriteDto>> PostUserFavorite(int userId, CreateUserFavoriteDto favoriteDto)
    {
        try
        {
            var createdFavorite = await _userService.AddUserFavoriteAsync(userId, favoriteDto);
            return CreatedAtAction(nameof(GetUserFavorites), new { userId = userId }, createdFavorite);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/favorites/{poemId}")]
    public async Task<IActionResult> DeleteUserFavorite(int userId, int poemId)
    {
        try
        {
            await _userService.RemoveUserFavoriteAsync(userId, poemId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/favorites/{poemId}")]
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
}