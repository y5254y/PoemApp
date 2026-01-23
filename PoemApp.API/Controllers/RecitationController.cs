using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Infrastructure.Data;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecitationController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecitationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult StartRecitation([FromBody] CreateRecitationDto dto)
    {
        var recitation = new UserRecitation
        {
            UserId = 1, // Replace with actual user ID from auth context
            PoemId = dto.PoemId,
            Notes = dto.Notes
        };

        _context.UserRecitations.Add(recitation);
        _context.SaveChanges();

        return Ok(recitation.Id);
    }

    [HttpGet("my")]
    public IActionResult GetMyRecitations()
    {
        var recitations = _context.UserRecitations
            .Where(r => r.UserId == 1) // Replace with actual user ID from auth context
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

    [HttpGet("{id}/reviews")]
    public IActionResult GetReviews(int id)
    {
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
            })
            .ToList();

        return Ok(reviews);
    }

    [HttpPost("{id}/reviews")]
    public IActionResult CompleteReview(int id, [FromBody] CreateReviewDto dto)
    {
        var review = _context.RecitationReviews.FirstOrDefault(rr => rr.Id == id);
        if (review == null) return NotFound();

        review.ActualReviewTime = DateTime.UtcNow;
        review.Status = Core.Enums.ReviewStatus.Completed;
        review.QualityRating = dto.QualityRating;
        review.Notes = dto.Notes;

        _context.SaveChanges();

        return Ok();
    }

    [HttpGet("due")]
    public IActionResult GetDueReviews()
    {
        var now = DateTime.UtcNow;
        var dueReviews = _context.RecitationReviews
            .Where(rr => rr.ScheduledTime <= now && rr.Status == Core.Enums.ReviewStatus.Pending)
            .Select(rr => new ReviewDto
            {
                Id = rr.Id,
                ScheduledTime = rr.ScheduledTime,
                Status = rr.Status.ToString()
            })
            .ToList();

        return Ok(dueReviews);
    }
}
