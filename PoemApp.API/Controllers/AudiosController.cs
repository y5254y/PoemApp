// AudioController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudiosController : ControllerBase
{
    private readonly IAudioService _audioService;

    public AudiosController(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AudioDto>>> GetAudios()
    {
        var audios = await _audioService.GetAllAudiosAsync();
        return Ok(audios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AudioDetailDto>> GetAudio(int id)
    {
        var audio = await _audioService.GetAudioByIdAsync(id);
        if (audio == null)
        {
            return NotFound();
        }
        return Ok(audio);
    }

    [HttpGet("poem/{poemId}")]
    public async Task<ActionResult<IEnumerable<AudioDto>>> GetAudiosByPoem(int poemId)
    {
        var audios = await _audioService.GetAudiosByPoemIdAsync(poemId);
        return Ok(audios);
    }

    [HttpGet("uploader/{uploaderId}")]
    public async Task<ActionResult<IEnumerable<AudioDto>>> GetAudiosByUploader(int uploaderId)
    {
        var audios = await _audioService.GetAudiosByUploaderIdAsync(uploaderId);
        return Ok(audios);
    }

    [HttpPost]
    public async Task<ActionResult<AudioDto>> PostAudio(CreateAudioDto audioDto)
    {
        try
        {
            var createdAudio = await _audioService.AddAudioAsync(audioDto);
            return CreatedAtAction(nameof(GetAudio), new { id = createdAudio.Id }, createdAudio);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAudio(int id, UpdateAudioDto audioDto)
    {
        try
        {
            await _audioService.UpdateAudioAsync(id, audioDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAudio(int id)
    {
        try
        {
            await _audioService.DeleteAudioAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 评分相关端点
    [HttpGet("{audioId}/ratings")]
    public async Task<ActionResult<IEnumerable<AudioRatingDto>>> GetAudioRatings(int audioId)
    {
        var ratings = await _audioService.GetAudioRatingsAsync(audioId);
        return Ok(ratings);
    }

    [HttpGet("{audioId}/ratings/my")]
    public async Task<ActionResult<AudioRatingDto>> GetMyAudioRating(int audioId)
    {
        try
        {
            int userId = GetCurrentUserId();
            var rating = await _audioService.GetUserAudioRatingAsync(audioId, userId);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{audioId}/ratings")]
    public async Task<ActionResult<AudioRatingDto>> PostAudioRating(int audioId, CreateAudioRatingDto ratingDto)
    {
        try
        {
            int userId = GetCurrentUserId();
            var createdRating = await _audioService.AddAudioRatingAsync(audioId, ratingDto, userId);
            return CreatedAtAction(nameof(GetMyAudioRating), new { audioId = audioId }, createdRating);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("ratings/{ratingId}")]
    public async Task<IActionResult> PutAudioRating(int ratingId, UpdateAudioRatingDto ratingDto)
    {
        try
        {
            int userId = GetCurrentUserId();
            await _audioService.UpdateAudioRatingAsync(ratingId, ratingDto, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpDelete("ratings/{ratingId}")]
    public async Task<IActionResult> DeleteAudioRating(int ratingId)
    {
        try
        {
            int userId = GetCurrentUserId();
            await _audioService.DeleteAudioRatingAsync(ratingId, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    // 辅助方法：从认证中获取当前用户ID
    private int GetCurrentUserId()
    {
        // 这里需要根据你的认证系统实现
        // 例如，如果你使用JWT，可以从Claims中获取用户ID
        // 这里返回一个示例值，实际应用中需要替换为真实的用户ID获取逻辑
        return 1;
    }
}