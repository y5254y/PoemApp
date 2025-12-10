using Microsoft.EntityFrameworkCore;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Infrastructure.Data;

namespace PoemApp.Infrastructure.Services;

public class QuoteService : IQuoteService
{
    private readonly AppDbContext _context;

    public QuoteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuoteDto>> GetAllQuotesAsync()
    {
        return await _context.Quotes
            .Include(q => q.Author)
            .Include(q => q.Poem)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Content = q.Content,
                AuthorId = q.AuthorId,
                AuthorName = q.Author != null ? q.Author.Name : null,
                PoemId = q.PoemId,
                PoemTitle = q.Poem != null ? q.Poem.Title : null,
                Source = q.Source,
                Translation = q.Translation,
                Annotation = q.Annotation,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<QuoteDto?> GetQuoteByIdAsync(int id)
    {
        var q = await _context.Quotes
            .Include(x => x.Author)
            .Include(x => x.Poem)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (q == null) return null;

        return new QuoteDto
        {
            Id = q.Id,
            Content = q.Content,
            AuthorId = q.AuthorId,
            AuthorName = q.Author != null ? q.Author.Name : null,
            PoemId = q.PoemId,
            PoemTitle = q.Poem != null ? q.Poem.Title : null,
            Source = q.Source,
            Translation = q.Translation,
            Annotation = q.Annotation,
            CreatedAt = q.CreatedAt
        };
    }

    public async Task<QuoteDto> AddQuoteAsync(CreateQuoteDto dto)
    {
        // If provided, validate foreign keys
        if (dto.AuthorId.HasValue)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId.Value);
            if (!authorExists) throw new ArgumentException("Author not found");
        }

        if (dto.PoemId.HasValue)
        {
            var poemExists = await _context.Poems.AnyAsync(p => p.Id == dto.PoemId.Value);
            if (!poemExists) throw new ArgumentException("Poem not found");
        }

        var quote = new Quote
        {
            Content = HtmlSanitizerHelper.SanitizeHtml(dto.Content),
            AuthorId = dto.AuthorId,
            PoemId = dto.PoemId,
            Source = HtmlSanitizerHelper.SanitizeHtml(dto.Source),
            Translation = HtmlSanitizerHelper.SanitizeHtml(dto.Translation),
            Annotation = HtmlSanitizerHelper.SanitizeHtml(dto.Annotation),
            CreatedAt = DateTime.UtcNow
        };

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        return await GetQuoteByIdAsync(quote.Id) ?? throw new InvalidOperationException("Failed to retrieve created quote");
    }

    public async Task UpdateQuoteAsync(int id, UpdateQuoteDto dto)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) throw new ArgumentException("Quote not found");

        if (dto.AuthorId.HasValue)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId.Value);
            if (!authorExists) throw new ArgumentException("Author not found");
        }

        if (dto.PoemId.HasValue)
        {
            var poemExists = await _context.Poems.AnyAsync(p => p.Id == dto.PoemId.Value);
            if (!poemExists) throw new ArgumentException("Poem not found");
        }

        quote.Content = HtmlSanitizerHelper.SanitizeHtml(dto.Content);
        quote.AuthorId = dto.AuthorId;
        quote.PoemId = dto.PoemId;
        quote.Source = HtmlSanitizerHelper.SanitizeHtml(dto.Source);
        quote.Translation = HtmlSanitizerHelper.SanitizeHtml(dto.Translation);
        quote.Annotation = HtmlSanitizerHelper.SanitizeHtml(dto.Annotation);

        _context.Quotes.Update(quote);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteQuoteAsync(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) throw new ArgumentException("Quote not found");

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<QuoteDto>> GetQuotesByAuthorAsync(int authorId)
    {
        return await _context.Quotes
            .Include(q => q.Author)
            .Include(q => q.Poem)
            .Where(q => q.AuthorId == authorId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Content = q.Content,
                AuthorId = q.AuthorId,
                AuthorName = q.Author != null ? q.Author.Name : null,
                PoemId = q.PoemId,
                PoemTitle = q.Poem != null ? q.Poem.Title : null,
                Source = q.Source,
                Translation = q.Translation,
                Annotation = q.Annotation,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<QuoteDto>> GetQuotesByPoemAsync(int poemId)
    {
        return await _context.Quotes
            .Include(q => q.Author)
            .Include(q => q.Poem)
            .Where(q => q.PoemId == poemId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Content = q.Content,
                AuthorId = q.AuthorId,
                AuthorName = q.Author != null ? q.Author.Name : null,
                PoemId = q.PoemId,
                PoemTitle = q.Poem != null ? q.Poem.Title : null,
                Source = q.Source,
                Translation = q.Translation,
                Annotation = q.Annotation,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }
}