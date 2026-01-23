using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Infrastructure.Data;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchievementController : ControllerBase
{
    private readonly AppDbContext _context;

    public AchievementController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllAchievements()
    {
        var achievements = _context.Achievements
            .Select(a => new AchievementDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Type = a.Type.ToString(),
                TargetValue = a.TargetValue,
                RewardPoints = a.RewardPoints,
                Level = a.Level,
                IsHidden = a.IsHidden
            })
            .ToList();

        return Ok(achievements);
    }

    [HttpGet("my")]
    public IActionResult GetMyAchievements()
    {
        var userAchievements = _context.UserAchievements
            .Where(ua => ua.UserId == 1) // Replace with actual user ID from auth context
            .Select(ua => new UserAchievementDto
            {
                Id = ua.Id,
                AchievementName = ua.Achievement.Name,
                AchievedAt = ua.AchievedAt,
                CurrentValue = ua.CurrentValue,
                RewardClaimed = ua.RewardClaimed
            })
            .ToList();

        return Ok(userAchievements);
    }

    [HttpPost("{id}/claim")]
    public IActionResult ClaimAchievementReward(int id)
    {
        var userAchievement = _context.UserAchievements.FirstOrDefault(ua => ua.Id == id);
        if (userAchievement == null || userAchievement.RewardClaimed) return NotFound();

        userAchievement.RewardClaimed = true;
        userAchievement.RewardClaimedAt = DateTime.UtcNow;

        _context.SaveChanges();

        return Ok();
    }

    [HttpGet("progress")]
    public IActionResult GetAchievementProgress()
    {
        // Example: Calculate progress for achievements
        return Ok();
    }
}
