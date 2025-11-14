using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication by default for all endpoints in this controller
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategoriesPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await _categoryService.GetCategoriesPagedAsync(page, pageSize, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    // Only Admin role can create categories
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> PostCategory(CreateCategoryDto categoryDto)
    {
        try
        {
            var createdCategory = await _categoryService.AddCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Only Admin role can update categories
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutCategory(int id, UpdateCategoryDto categoryDto)
    {
        try
        {
            await _categoryService.UpdateCategoryAsync(id, categoryDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Only Admin role can delete categories
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Only Admin role can add/remove poems to categories
    [HttpPost("{categoryId}/poems/{poemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPoemToCategory(int categoryId, int poemId)
    {
        try
        {
            await _categoryService.AddPoemToCategoryAsync(poemId, categoryId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{categoryId}/poems/{poemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemovePoemFromCategory(int categoryId, int poemId)
    {
        try
        {
            await _categoryService.RemovePoemFromCategoryAsync(poemId, categoryId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}