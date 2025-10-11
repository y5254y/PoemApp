using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }
        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> PostAuthor(CreateAuthorDto authorDto)
    {
        try
        {
            var createdAuthor = await _authorService.AddAuthorAsync(authorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, UpdateAuthorDto authorDto)
    {
        try
        {
            await _authorService.UpdateAuthorAsync(id, authorDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
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