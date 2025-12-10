using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetAll()
    {
        var items = await _quoteService.GetAllQuotesAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuoteDto>> Get(int id)
    {
        var item = await _quoteService.GetQuoteByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("byauthor/{authorId}")]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetByAuthor(int authorId)
    {
        var items = await _quoteService.GetQuotesByAuthorAsync(authorId);
        return Ok(items);
    }

    [HttpGet("bypoem/{poemId}")]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetByPoem(int poemId)
    {
        var items = await _quoteService.GetQuotesByPoemAsync(poemId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<QuoteDto>> Post(CreateQuoteDto dto)
    {
        try
        {
            var created = await _quoteService.AddQuoteAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, UpdateQuoteDto dto)
    {
        try
        {
            await _quoteService.UpdateQuoteAsync(id, dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _quoteService.DeleteQuoteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
