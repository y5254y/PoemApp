using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PoemApp.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecitationController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecitationController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        // Try multiple claim name variations
        var claimNames = new[] 
        { 
            "sub", 
            System.Security.Claims.ClaimTypes.NameIdentifier,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            "userId", 
            "id",
            "user_id"
        };

        foreach (var claimName in claimNames)
        {
            var claimValue = User.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out var userId) && userId > 0)
            {
                return userId;
            }
        }

        // Fallback: log all claims for debugging
        var allClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
        throw new UnauthorizedAccessException($"User ID not found in token. Available claims: {allClaims}");
    }

    [HttpPost]
    public IActionResult StartRecitation([FromBody] CreateRecitationDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();

            var recitation = new UserRecitation
            {
                UserId = userId,
                PoemId = dto.PoemId,
                Notes = dto.Notes,
                NextReviewTime = DateTime.UtcNow.AddDays(1)
            };

            _context.UserRecitations.Add(recitation);
            _context.SaveChanges();

            // Create first review schedule
            var firstReview = new RecitationReview
            {
                UserRecitationId = recitation.Id,
                ScheduledTime = recitation.NextReviewTime.Value,
                ReviewRound = 1,
                Status = ReviewStatus.Pending
            };
            _context.RecitationReviews.Add(firstReview);
            _context.SaveChanges();

            return Ok(recitation.Id);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User ID not found in token");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my")]
    public IActionResult GetMyRecitations()
    {
        try
        {
            var userId = GetCurrentUserId();

            var recitations = _context.UserRecitations
                .Where(r => r.UserId == userId)
                .Select(r => new RecitationDto
                {
                    Id = r.Id,
                    PoemId = r.PoemId,
                    PoemTitle = r.Poem.Title,
                    Status = r.Status.ToString(),
                    NextReviewTime = r.NextReviewTime,
                    Proficiency = r.Proficiency
                })
                .ToList();

            return Ok(recitations);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User ID not found in token");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/reviews")]
    public IActionResult GetReviews(int id)
    {
        try
        {
            var userId = GetCurrentUserId();

            // Verify ownership
            var recitation = _context.UserRecitations.FirstOrDefault(r => r.Id == id);
            if (recitation == null || recitation.UserId != userId)
            {
                return Forbid();
            }

            var reviews = _context.RecitationReviews
                .Where(rr => rr.UserRecitationId == id)
                .Select(rr => new ReviewDto
                {
                    Id = rr.Id,
                    ScheduledTime = rr.ScheduledTime,
                    ActualReviewTime = rr.ActualReviewTime,
                    Status = rr.Status.ToString(),
                    QualityRating = rr.QualityRating,
                    ReviewRound = rr.ReviewRound
                    ,
                    UserRecitationId = rr.UserRecitationId,
                    PoemId = rr.UserRecitation.PoemId,
                    PoemTitle = rr.UserRecitation.Poem.Title
                })
                .ToList();

            return Ok(reviews);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User ID not found in token");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/reviews")]
    public IActionResult CompleteReview(int id, [FromBody] CreateReviewDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();

            var review = _context.RecitationReviews
                .Include(r => r.UserRecitation)
                .FirstOrDefault(rr => rr.Id == id);
            if (review == null) return NotFound();

            // Verify ownership
            if (review.UserRecitation.UserId != userId)
            {
                return Forbid();
            }

            var now = DateTime.UtcNow;
            review.ActualReviewTime = now;
            review.Status = ReviewStatus.Completed;
            review.QualityRating = dto.QualityRating;
            review.Notes = dto.Notes;

            // Update UserRecitation
            var recitation = review.UserRecitation;
            recitation.LastReviewTime = now;
            recitation.ReviewCount++;
            recitation.UpdatedAt = now;

            // Update proficiency based on quality rating (1-5)
            // Quality: 1=完全忘记, 2=模糊, 3=能想起, 4=熟练, 5=完美
            int qualityBonus = (dto.QualityRating - 1) * 5; // 0, 5, 10, 15, 20
            recitation.Proficiency = Math.Min(100, recitation.Proficiency + qualityBonus);

            // Calculate next review time based on Ebbinghaus curve
            // Review intervals: 1, 2, 4, 7, 15, 30 days
            int[] intervals = { 1, 2, 4, 7, 15, 30 };
            int nextInterval = review.ReviewRound < intervals.Length 
                ? intervals[review.ReviewRound] 
                : 30;
            
            recitation.NextReviewTime = now.AddDays(nextInterval);

            // Update status based on proficiency and review count
            if (recitation.Proficiency >= 80 && recitation.ReviewCount >= 3)
            {
                recitation.Status = RecitationStatus.Mastered;
            }
            else if (recitation.Proficiency >= 50)
            {
                recitation.Status = RecitationStatus.NeedReview;
            }
            else
            {
                recitation.Status = RecitationStatus.Learning;
            }

            // Schedule next review if not mastered
            if (recitation.Status != RecitationStatus.Mastered)
            {
                var nextReview = new RecitationReview
                {
                    UserRecitationId = recitation.Id,
                    ScheduledTime = recitation.NextReviewTime.Value,
                    ReviewRound = review.ReviewRound + 1,
                    Status = ReviewStatus.Pending
                };
                _context.RecitationReviews.Add(nextReview);
            }

            _context.SaveChanges();

            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User ID not found in token");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("due")]
    public IActionResult GetDueReviews()
    {
        try
        {
            var userId = GetCurrentUserId();
            var now = DateTime.UtcNow;

            var dueReviews = _context.RecitationReviews
                .Where(rr => rr.ScheduledTime <= now && rr.Status == ReviewStatus.Pending && rr.UserRecitation.UserId == userId)
                .Select(rr => new ReviewDto
                {
                    Id = rr.Id,
                    ScheduledTime = rr.ScheduledTime,
                    Status = rr.Status.ToString()
                    ,
                    UserRecitationId = rr.UserRecitationId,
                    PoemId = rr.UserRecitation.PoemId,
                    PoemTitle = rr.UserRecitation.Poem.Title
                })
                .ToList();

            return Ok(dueReviews);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User ID not found in token");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
