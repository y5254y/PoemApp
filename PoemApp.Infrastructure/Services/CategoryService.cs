using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions; // for GetDisplayName on enums used in PoemDto mapping

namespace PoemApp.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Include(c => c.Poems)
                .ThenInclude(pc => pc.Poem)
                    .ThenInclude(p => p.Author)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Group = c.Group,
                ParentId = c.ParentId,
                Poems = c.Poems.Select(pc => new PoemDto
                {
                    Id = pc.Poem.Id,
                    Title = pc.Poem.Title,
                    Content = pc.Poem.Content,
                    AuthorId = pc.Poem.AuthorId,
                    AuthorName = pc.Poem.Author.Name,
                    Dynasty = pc.Poem.Author.Dynasty,
                    DynastyDisplayName = pc.Poem.Author.Dynasty.GetDisplayName(),
                    Background = pc.Poem.Background ?? string.Empty,
                    Translation = pc.Poem.Translation ?? string.Empty,
                    Annotation = pc.Poem.Annotation ?? string.Empty,
                    Appreciation = pc.Poem.Appreciation ?? string.Empty
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Poems)
                .ThenInclude(pc => pc.Poem)
                    .ThenInclude(p => p.Author)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Group = category.Group,
            ParentId = category.ParentId,
            Poems = category.Poems.Select(pc => new PoemDto
            {
                Id = pc.Poem.Id,
                Title = pc.Poem.Title,
                Content = pc.Poem.Content,
                AuthorId = pc.Poem.AuthorId,
                AuthorName = pc.Poem.Author.Name,
                Dynasty = pc.Poem.Author.Dynasty,
                DynastyDisplayName = pc.Poem.Author.Dynasty.GetDisplayName(),
                Background = pc.Poem.Background ?? string.Empty,
                Translation = pc.Poem.Translation ?? string.Empty,
                Annotation = pc.Poem.Annotation ?? string.Empty,
                Appreciation = pc.Poem.Appreciation ?? string.Empty
            }).ToList()
        };
    }

    public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            Group = categoryDto.Group,
            ParentId = categoryDto.ParentId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryByIdAsync(category.Id) ?? throw new InvalidOperationException("Failed to load created category");
    }

    public async Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;
        category.Group = categoryDto.Group;
        category.ParentId = categoryDto.ParentId;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        // Remove any PoemCategory relationships first to avoid FK constraint issues
        var relations = _context.PoemCategories.Where(pc => pc.CategoryId == id);
        _context.PoemCategories.RemoveRange(relations);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task AddPoemToCategoryAsync(int poemId, int categoryId)
    {
        // Check if poem exists
        var poem = await _context.Poems.FindAsync(poemId);
        if (poem == null)
        {
            throw new ArgumentException("Poem not found");
        }

        // Check if category exists
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        // Check if relationship already exists
        var existingRelationship = await _context.PoemCategories
            .FirstOrDefaultAsync(pc => pc.PoemId == poemId && pc.CategoryId == categoryId);

        if (existingRelationship != null)
        {
            throw new ArgumentException("Poem is already in this category");
        }

        var poemCategory = new PoemCategory
        {
            PoemId = poemId,
            CategoryId = categoryId
        };

        _context.PoemCategories.Add(poemCategory);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePoemFromCategoryAsync(int poemId, int categoryId)
    {
        var poemCategory = await _context.PoemCategories
            .FirstOrDefaultAsync(pc => pc.PoemId == poemId && pc.CategoryId == categoryId);

        if (poemCategory == null)
        {
            throw new ArgumentException("Poem is not in this category");
        }

        _context.PoemCategories.Remove(poemCategory);
        await _context.SaveChangesAsync();
    }
}