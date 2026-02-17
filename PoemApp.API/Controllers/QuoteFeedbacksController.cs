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
public class QuoteFeedbacksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public QuoteFeedbacksController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<QuoteFeedbackDto>>> GetFeedbacks(
        [FromQuery] int? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.QuoteFeedbacks
            .Include(f => f.Quote)
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
            .Select(f => new QuoteFeedbackDto
            {
                Id = f.Id,
                QuoteId = f.QuoteId,
                QuoteContent = f.Quote.Content,
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
    public async Task<ActionResult<QuoteFeedbackDto>> GetFeedback(int id)
    {
        var feedback = await _context.QuoteFeedbacks
            .Include(f => f.Quote)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (feedback == null)
        {
            return NotFound();
        }

        var dto = new QuoteFeedbackDto
        {
            Id = feedback.Id,
            QuoteId = feedback.QuoteId,
            QuoteContent = feedback.Quote.Content,
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
    public async Task<ActionResult<QuoteFeedbackDto>> CreateFeedback(CreateQuoteFeedbackDto dto)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Unable to determine user ID");
        }

        var quote = await _context.Quotes.FindAsync(dto.QuoteId);
        if (quote == null)
        {
            return BadRequest("Quote not found");
        }

        var feedback = new QuoteFeedback
        {
            QuoteId = dto.QuoteId,
            UserId = userId,
            Content = dto.Content,
            Category = dto.Category
        };

        _context.QuoteFeedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        var result = new QuoteFeedbackDto
        {
            Id = feedback.Id,
            QuoteId = feedback.QuoteId,
            QuoteContent = quote.Content,
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
        var feedback = await _context.QuoteFeedbacks.FindAsync(id);
        if (feedback == null)
        {
            return NotFound();
        }

        feedback.Status = dto.Status;
        feedback.AdminReply = dto.AdminReply;
        feedback.ResolvedAt = dto.Status != Core.Enums.FeedbackStatus.Pending ? DateTime.UtcNow : null;

        _context.QuoteFeedbacks.Update(feedback);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFeedback(int id)
    {
        var feedback = await _context.QuoteFeedbacks.FindAsync(id);
        if (feedback == null)
        {
            return NotFound();
        }

        _context.QuoteFeedbacks.Remove(feedback);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
