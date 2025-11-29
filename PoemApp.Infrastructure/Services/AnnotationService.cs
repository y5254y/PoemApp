// AnnotationService.cs
using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;

namespace PoemApp.Infrastructure.Services;

public class AnnotationService : IAnnotationService
{
    private readonly AppDbContext _context;

    public AnnotationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AnnotationDto>> GetAllAnnotationsAsync()
    {
        return await _context.Annotations
            .Include(a => a.Poem)
            .Include(a => a.User)
            .Select(a => new AnnotationDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                UserId = a.UserId,
                UserName = a.User.Username,
                HighlightText = a.HighlightText,
                Comment = a.Comment,
                StartIndex = a.StartIndex,
                EndIndex = a.EndIndex,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AnnotationDto> GetAnnotationByIdAsync(int id)
    {
        var annotation = await _context.Annotations
            .Include(a => a.Poem)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (annotation == null) return null;

        return new AnnotationDto
        {
            Id = annotation.Id,
            PoemId = annotation.PoemId,
            PoemTitle = annotation.Poem.Title,
            UserId = annotation.UserId,
            UserName = annotation.User.Username,
            HighlightText = annotation.HighlightText,
            Comment = annotation.Comment,
            StartIndex = annotation.StartIndex,
            EndIndex = annotation.EndIndex,
            CreatedAt = annotation.CreatedAt
        };
    }

    public async Task<IEnumerable<AnnotationDto>> GetAnnotationsByPoemIdAsync(int poemId)
    {
        return await _context.Annotations
            .Include(a => a.Poem)
            .Include(a => a.User)
            .Where(a => a.PoemId == poemId)
            .Select(a => new AnnotationDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                UserId = a.UserId,
                UserName = a.User.Username,
                HighlightText = a.HighlightText,
                Comment = a.Comment,
                StartIndex = a.StartIndex,
                EndIndex = a.EndIndex,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AnnotationDto>> GetAnnotationsByUserIdAsync(int userId)
    {
        return await _context.Annotations
            .Include(a => a.Poem)
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .Select(a => new AnnotationDto
            {
                Id = a.Id,
                PoemId = a.PoemId,
                PoemTitle = a.Poem.Title,
                UserId = a.UserId,
                UserName = a.User.Username,
                HighlightText = a.HighlightText,
                Comment = a.Comment,
                StartIndex = a.StartIndex,
                EndIndex = a.EndIndex,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AnnotationDto> AddAnnotationAsync(CreateAnnotationDto annotationDto, int userId)
    {
        // 检查诗文是否存在
        var poem = await _context.Poems.FindAsync(annotationDto.PoemId);
        if (poem == null)
        {
            throw new ArgumentException("Poem not found");
        }

        // 检查用户是否存在
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var annotation = new Annotation
        {
            PoemId = annotationDto.PoemId,
            UserId = userId,
            HighlightText = annotationDto.HighlightText,
            Comment = annotationDto.Comment,
            StartIndex = annotationDto.StartIndex,
            EndIndex = annotationDto.EndIndex,
            CreatedAt = DateTime.UtcNow
        };

        _context.Annotations.Add(annotation);
        await _context.SaveChangesAsync();

        // 返回创建的标注
        return await GetAnnotationByIdAsync(annotation.Id);
    }

    public async Task UpdateAnnotationAsync(int id, UpdateAnnotationDto annotationDto, int userId)
    {
        var annotation = await _context.Annotations.FindAsync(id);
        if (annotation == null)
        {
            throw new ArgumentException("Annotation not found");
        }

        // 检查用户是否有权限修改此标注
        if (annotation.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own annotations");
        }

        annotation.HighlightText = annotationDto.HighlightText;
        annotation.Comment = annotationDto.Comment;
        annotation.StartIndex = annotationDto.StartIndex;
        annotation.EndIndex = annotationDto.EndIndex;

        _context.Annotations.Update(annotation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnnotationAsync(int id, int userId)
    {
        var annotation = await _context.Annotations.FindAsync(id);
        if (annotation == null)
        {
            throw new ArgumentException("Annotation not found");
        }

        // 检查用户是否有权限删除此标注
        if (annotation.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own annotations");
        }

        _context.Annotations.Remove(annotation);
        await _context.SaveChangesAsync();
    }
}