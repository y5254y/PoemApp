using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PoemsController : ControllerBase
{
    private readonly IPoemService _poemService;

    public PoemsController(IPoemService poemService)
    {
        _poemService = poemService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PoemDto>>> GetPoems()
    {
        var poems = await _poemService.GetAllPoemsAsync();
        return Ok(poems);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PoemDto>> GetPoem(int id)
    {
        var poem = await _poemService.GetPoemByIdAsync(id);

        if (poem == null)
        {
            return NotFound();
        }

        return Ok(poem);
    }

    [HttpGet("category/{categoryName}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PoemDto>>> GetPoemsByCategory(string categoryName)
    {
        var poems = await _poemService.GetPoemsByCategoryAsync(categoryName);
        return Ok(poems);
    }

    [HttpGet("author/{authorId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PoemDto>>> GetPoemsByAuthor(int authorId)
    {
        var poems = await _poemService.GetPoemsByAuthorAsync(authorId);
        return Ok(poems);
    }

    [HttpPost]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<PoemDto>> PostPoem(CreatePoemDto poemDto)
    {
        try
        {
            var createdPoem = await _poemService.AddPoemAsync(poemDto);
            return CreatedAtAction(nameof(GetPoem), new { id = createdPoem.Id }, createdPoem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutPoem(int id, UpdatePoemDto poemDto)
    {
        try
        {
            await _poemService.UpdatePoemAsync(id, poemDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePoem(int id)
    {
        try
        {
            await _poemService.DeletePoemAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("{poemId}/categories/{categoryId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategoryToPoem(int poemId, int categoryId)
    {
        try
        {
            await _poemService.AddCategoryToPoemAsync(poemId, categoryId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{poemId}/categories/{categoryId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveCategoryFromPoem(int poemId, int categoryId)
    {
        try
        {
            await _poemService.RemoveCategoryFromPoemAsync(poemId, categoryId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}