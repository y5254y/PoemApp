using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions; // for GetDisplayName on enums used in PoemDto mapping
using System.Text.RegularExpressions;
using System.Net;
using Ganss.Xss;
using PoemApp.Core.Enums;

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
        var cats = await _context.Categories
            .Include(c => c.Poems)
                .ThenInclude(pc => pc.Poem)
                    .ThenInclude(p => p.Author)
            .Include(c => c.Parent)
            .ToListAsync();

        return cats.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Group = c.Group,
            ParentId = c.ParentId,
            ParentName = c.Parent?.Name,
            SortOrder = c.SortOrder,
            IsEnabled = c.IsEnabled,
            IsLeaf = c.IsLeaf,
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
        }).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Poems)
                .ThenInclude(pc => pc.Poem)
                    .ThenInclude(p => p.Author)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Group = category.Group,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            SortOrder = category.SortOrder,
            IsEnabled = category.IsEnabled,
            IsLeaf = category.IsLeaf,
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

    // helper to get all categories as DTO for selection UI
    public async Task<List<CategoryDto>> GetAllCategoryOptionsAsync()
    {
        var cats = await _context.Categories.Include(c => c.Parent).ToListAsync();
        return cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, ParentId = c.ParentId, ParentName = c.Parent?.Name }).ToList();
    }

    public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = HtmlSanitizerHelper.SanitizeHtml(categoryDto.Description),
            Group = categoryDto.Group,
            ParentId = categoryDto.ParentId
            ,SortOrder = categoryDto.SortOrder
            ,IsEnabled = categoryDto.IsEnabled
            ,IsLeaf = categoryDto.IsLeaf
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

        // validate parent is not itself or its descendant
        if (categoryDto.ParentId.HasValue)
        {
            if (categoryDto.ParentId.Value == id) throw new ArgumentException("Category cannot be its own parent");
            var descendants = await GetDescendantIdsAsync(id);
            if (descendants.Contains(categoryDto.ParentId.Value)) throw new ArgumentException("Cannot set parent to a descendant category");
        }

        category.Name = categoryDto.Name;
        category.Description = HtmlSanitizerHelper.SanitizeHtml(categoryDto.Description);
        category.Group = categoryDto.Group;
        category.ParentId = categoryDto.ParentId;
        category.SortOrder = categoryDto.SortOrder;
        category.IsEnabled = categoryDto.IsEnabled;
        category.IsLeaf = categoryDto.IsLeaf;

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

    public async Task<PagedResult<CategoryDto>> GetCategoriesPagedAsync(int page, int pageSize, string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            // search against name or enum display name
            query = query.Where(c => c.Name.Contains(s) || (c.Group != null && EF.Functions.Like(c.Group.ToString(), $"%{s}%")));
        }

        var total = await query.CountAsync();

        var items = await query
            .Include(c => c.Parent)
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Group = c.Group,
                ParentId = c.ParentId,
                ParentName = c.Parent != null ? c.Parent.Name : null,
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
            }).ToListAsync();

        return new PagedResult<CategoryDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    // get all descendant ids for cycle check
    private async Task<HashSet<int>> GetDescendantIdsAsync(int id)
    {
        var result = new HashSet<int>();
        var children = await _context.Categories.Where(c => c.ParentId == id).Select(c => c.Id).ToListAsync();
        foreach (var childId in children)
        {
            if (result.Add(childId))
            {
                var sub = await GetDescendantIdsAsync(childId);
                foreach (var s in sub) result.Add(s);
            }
        }
        return result;
    }
}