﻿using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions;
using PoemApp.Core.Enums; // 添加枚举命名空间

namespace PoemApp.Infrastructure.Services;

public class PoemService : IPoemService
{
    private readonly AppDbContext _context;

    public PoemService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PoemDto>> GetAllPoemsAsync()
    {
        return await _context.Poems
            .Include(p => p.Author)
            .Include(p => p.Categories)
                .ThenInclude(pc => pc.Category)
            .Select(p => new PoemDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.Name,
                // 使用枚举而不是字符串
                Dynasty = p.Author.Dynasty,
                // 使用扩展方法获取友好的显示名称
                DynastyDisplayName = p.Author.Dynasty.GetDisplayName(),
                Background = p.Background ?? string.Empty,
                Translation = p.Translation ?? string.Empty,
                Annotation = p.Annotation ?? string.Empty,
                Categories = p.Categories.Select(pc => pc.Category.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<PoemDto> GetPoemByIdAsync(int id)
    {
        var poem = await _context.Poems
            .Include(p => p.Author)
            .Include(p => p.Categories)
                .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (poem == null) return null;

        return new PoemDto
        {
            Id = poem.Id,
            Title = poem.Title,
            Content = poem.Content,
            AuthorId = poem.AuthorId,
            AuthorName = poem.Author.Name,
            // 添加枚举字段
            Dynasty = poem.Author.Dynasty,
            DynastyDisplayName = poem.Author.Dynasty.GetDisplayName(),
            Background = poem.Background ?? string.Empty,
            Translation = poem.Translation ?? string.Empty,
            Annotation = poem.Annotation ?? string.Empty,
            Categories = poem.Categories.Select(pc => pc.Category.Name).ToList()
        };
    }

    public async Task<IEnumerable<PoemDto>> GetPoemsByCategoryAsync(string categoryName)
    {
        return await _context.Poems
            .Include(p => p.Author)
            .Include(p => p.Categories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.Categories.Any(pc => pc.Category.Name == categoryName))
            .Select(p => new PoemDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.Name,
                // 添加枚举字段
                Dynasty = p.Author.Dynasty,
                DynastyDisplayName = p.Author.Dynasty.GetDisplayName(),
                Background = p.Background ?? string.Empty,
                Translation = p.Translation ?? string.Empty,
                Annotation = p.Annotation ?? string.Empty,
                Categories = p.Categories.Select(pc => pc.Category.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PoemDto>> GetPoemsByAuthorAsync(int authorId)
    {
        return await _context.Poems
            .Include(p => p.Author)
            .Include(p => p.Categories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.AuthorId == authorId)
            .Select(p => new PoemDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.Name,
                // 添加枚举字段
                Dynasty = p.Author.Dynasty,
                DynastyDisplayName = p.Author.Dynasty.GetDisplayName(),
                Background = p.Background ?? string.Empty,
                Translation = p.Translation ?? string.Empty,
                Annotation = p.Annotation ?? string.Empty,
                Categories = p.Categories.Select(pc => pc.Category.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<PoemDto> AddPoemAsync(CreatePoemDto poemDto)
    {
        // Check if author exists
        var author = await _context.Authors.FindAsync(poemDto.AuthorId);
        if (author == null)
        {
            throw new ArgumentException("Author not found");
        }

        // Create new poem
        var poem = new Poem
        {
            Title = poemDto.Title,
            Content = poemDto.Content,
            AuthorId = poemDto.AuthorId,
            Background = poemDto.Background,
            Translation = poemDto.Translation,
            Annotation = poemDto.Annotation,
            Categories = new List<PoemCategory>()
        };

        // Add categories
        foreach (var categoryId in poemDto.CategoryIds)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                poem.Categories.Add(new PoemCategory
                {
                    CategoryId = categoryId,
                    Poem = poem
                });
            }
        }

        _context.Poems.Add(poem);
        await _context.SaveChangesAsync();

        // Return the created poem
        return await GetPoemByIdAsync(poem.Id);
    }

    public async Task UpdatePoemAsync(int id, UpdatePoemDto poemDto)
    {
        var poem = await _context.Poems
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (poem == null)
        {
            throw new ArgumentException("Poem not found");
        }

        // Update basic properties
        poem.Title = poemDto.Title;
        poem.Content = poemDto.Content;
        poem.AuthorId = poemDto.AuthorId;
        poem.Background = poemDto.Background;
        poem.Translation = poemDto.Translation;
        poem.Annotation = poemDto.Annotation;

        // Update categories
        poem.Categories.Clear();
        foreach (var categoryId in poemDto.CategoryIds)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                poem.Categories.Add(new PoemCategory
                {
                    CategoryId = categoryId,
                    PoemId = id
                });
            }
        }

        _context.Poems.Update(poem);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePoemAsync(int id)
    {
        var poem = await _context.Poems.FindAsync(id);
        if (poem == null)
        {
            throw new ArgumentException("Poem not found");
        }

        _context.Poems.Remove(poem);
        await _context.SaveChangesAsync();
    }





    // 添加专门的诗文分类管理方法
    public async Task UpdatePoemCategoriesAsync(int poemId, List<int> categoryIds)
    {
        var poem = await _context.Poems
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == poemId);

        if (poem == null) throw new ArgumentException("Poem not found");

        // 清除现有分类
        poem.Categories.Clear();

        // 添加新分类
        foreach (var categoryId in categoryIds)
        {
            if (await _context.Categories.AnyAsync(c => c.Id == categoryId))
            {
                poem.Categories.Add(new PoemCategory
                {
                    PoemId = poemId,
                    CategoryId = categoryId
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddCategoryToPoemAsync(int poemId, int categoryId)
    {
        // 检查关系是否已存在
        var exists = await _context.PoemCategories
            .AnyAsync(pc => pc.PoemId == poemId && pc.CategoryId == categoryId);

        if (exists) throw new ArgumentException("Category already assigned to poem");

        _context.PoemCategories.Add(new PoemCategory
        {
            PoemId = poemId,
            CategoryId = categoryId
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveCategoryFromPoemAsync(int poemId, int categoryId)
    {
        var poemCategory = await _context.PoemCategories
            .FirstOrDefaultAsync(pc => pc.PoemId == poemId && pc.CategoryId == categoryId);

        if (poemCategory == null) throw new ArgumentException("Category not assigned to poem");

        _context.PoemCategories.Remove(poemCategory);
        await _context.SaveChangesAsync();
    }
}