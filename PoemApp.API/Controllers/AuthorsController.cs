using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<AuthorDto>>> GetAuthors([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var authors = await _authorService.GetAllAuthorsAsync(page, pageSize, search);
        return Ok(authors);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        _logger.LogInformation("GetAuthor: id={Id} biography length={Len}", id, author.Biography?.Length ?? 0);
        return Ok(author);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthorDto>> PostAuthor(CreateAuthorDto authorDto)
    {
        try
        {
            _logger.LogInformation("PostAuthor: incoming biography length={Len}", authorDto.Biography?.Length ?? 0);
            var createdAuthor = await _authorService.AddAuthorAsync(authorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutAuthor(int id, UpdateAuthorDto authorDto)
    {
        try
        {
            _logger.LogInformation("PutAuthor: id={Id} incoming biography length={Len}", id, authorDto.Biography?.Length ?? 0);
            await _authorService.UpdateAuthorAsync(id, authorDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("relationships")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthorRelationshipDto>> PostAuthorRelationship(CreateAuthorRelationshipDto relationshipDto)
    {
        try
        {
            var createdRelationship = await _authorService.AddAuthorRelationshipAsync(relationshipDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdRelationship.FromAuthorId }, createdRelationship);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("relationships/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthorRelationship(int id)
    {
        try
        {
            await _authorService.RemoveAuthorRelationshipAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}