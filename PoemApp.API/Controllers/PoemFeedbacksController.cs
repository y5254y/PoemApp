using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PoemFeedbacksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PoemFeedbacksController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PoemFeedbackDto>>> GetFeedbacks(
        [FromQuery] int? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.PoemFeedbacks
            .Include(f => f.Poem)
            .Include(f => f.User)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(f => (int)f.Status == status.Value);
        }

        var total = await query.CountAsync();
        var feedbacks = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new PoemFeedbackDto
            {
                Id = f.Id,
                PoemId = f.PoemId,
                PoemTitle = f.Poem.Title,
                UserId = f.UserId,
                Username = f.User.Username,
                Content = f.Content,
                Category = f.Category,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                ResolvedAt = f.ResolvedAt,
                AdminReply = f.AdminReply
            })
            .ToListAsync();

        return Ok(new { total, data = feedbacks });
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PoemFeedbackDto>> GetFeedback(int id)
    {
        var feedback = await _context.PoemFeedbacks
            .Include(f => f.Poem)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (feedback == null)
        {
            return NotFound();
        }

        var dto = new PoemFeedbackDto
        {
            Id = feedback.Id,
            PoemId = feedback.PoemId,
            PoemTitle = feedback.Poem.Title,
            UserId = feedback.UserId,
            Username = feedback.User.Username,
            Content = feedback.Content,
            Category = feedback.Category,
            Status = feedback.Status,
            CreatedAt = feedback.CreatedAt,
            ResolvedAt = feedback.ResolvedAt,
            AdminReply = feedback.AdminReply
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PoemFeedbackDto>> CreateFeedback(CreatePoemFeedbackDto dto)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Unable to determine user ID");
        }

        var poem = await _context.Poems.FindAsync(dto.PoemId);
        if (poem == null)
        {
            return BadRequest("Poem not found");
        }

        var feedback = new PoemFeedback
        {
            PoemId = dto.PoemId,
            UserId = userId,
            Content = dto.Content,
            Category = dto.Category
        };

        _context.PoemFeedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        var result = new PoemFeedbackDto
        {
            Id = feedback.Id,
            PoemId = feedback.PoemId,
            PoemTitle = poem.Title,
            UserId = feedback.UserId,
            Content = feedback.Content,
            Category = feedback.Category,
            Status = feedback.Status,
            CreatedAt = feedback.CreatedAt
        };

        return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFeedback(int id, UpdateFeedbackStatusDto dto)
    {
        var feedback = await _context.PoemFeedbacks.FindAsync(id);
        if (feedback == null)
        {
            return NotFound();
        }

        feedback.Status = dto.Status;
        feedback.AdminReply = dto.AdminReply;
        feedback.ResolvedAt = dto.Status != Core.Enums.FeedbackStatus.Pending ? DateTime.UtcNow : null;

        _context.PoemFeedbacks.Update(feedback);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFeedback(int id)
    {
        var feedback = await _context.PoemFeedbacks.FindAsync(id);
        if (feedback == null)
        {
            return NotFound();
        }

        _context.PoemFeedbacks.Remove(feedback);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
