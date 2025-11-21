using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using PoemApp.Core.Extensions;

namespace PoemApp.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly AppDbContext _context;

    public AuthorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AuthorDto>> GetAllAuthorsAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var query = _context.Authors
            .Include(a => a.Relationships)
                .ThenInclude(r => r.ToAuthor)
            .Include(a => a.Poems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.ToLowerInvariant();
            query = query.Where(a => a.Name.ToLower().Contains(lowered) || (a.Biography ?? string.Empty).ToLower().Contains(lowered) || a.Dynasty.ToString().ToLower().Contains(lowered));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                Dynasty = a.Dynasty,
                DynastyDisplayName = a.Dynasty.GetDisplayName(),
                Biography = a.Biography,
                Relationships = a.Relationships.Select(r => new AuthorRelationshipDto
                {
                    Id = r.Id,
                    FromAuthorId = r.FromAuthorId,
                    FromAuthorName = r.FromAuthor.Name,
                    ToAuthorId = r.ToAuthorId,
                    ToAuthorName = r.ToAuthor.Name,
                    RelationshipType = r.RelationshipType,
                    RelationshipTypeDisplayName = r.RelationshipType.GetDisplayName()
                }).ToList(),
                Poems = a.Poems.Select(p => new PoemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name,
                    Dynasty = p.Author.Dynasty,
                    DynastyDisplayName = p.Author.Dynasty.GetDisplayName(),
                    Background = p.Background ?? string.Empty,
                    Translation = p.Translation ?? string.Empty,
                    Annotation = p.Annotation ?? string.Empty
                }).ToList()
            })
            .ToListAsync();

        return new PagedResult<AuthorDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AuthorDto> GetAuthorByIdAsync(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Relationships)
                .ThenInclude(r => r.ToAuthor)
            .Include(a => a.Poems)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
            throw new ArgumentException("Author not found");

        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Dynasty = author.Dynasty,
            DynastyDisplayName = author.Dynasty.GetDisplayName(),
            Biography = author.Biography,
            Relationships = author.Relationships.Select(r => new AuthorRelationshipDto
            {
                Id = r.Id,
                FromAuthorId = r.FromAuthorId,
                FromAuthorName = r.FromAuthor.Name,
                ToAuthorId = r.ToAuthorId,
                ToAuthorName = r.ToAuthor.Name,
                RelationshipType = r.RelationshipType,
                RelationshipTypeDisplayName = r.RelationshipType.GetDisplayName()
            }).ToList(),
            Poems = author.Poems.Select(p => new PoemDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.Name,
                Dynasty = p.Author.Dynasty,
                DynastyDisplayName = p.Author.Dynasty.GetDisplayName(),
                Background = p.Background ?? string.Empty,
                Translation = p.Translation ?? string.Empty,
                Annotation = p.Annotation ?? string.Empty
            }).ToList()
        };
    }

    public async Task<AuthorDto> AddAuthorAsync(CreateAuthorDto authorDto)
    {
        var author = new Author
        {
            Name = authorDto.Name,
            Dynasty = authorDto.Dynasty,
            Biography = HtmlSanitizerHelper.SanitizeHtml(authorDto.Biography)
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return await GetAuthorByIdAsync(author.Id);
    }

    public async Task UpdateAuthorAsync(int id, UpdateAuthorDto authorDto)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            throw new ArgumentException("Author not found");
        }

        author.Name = authorDto.Name;
        author.Dynasty = authorDto.Dynasty;
        author.Biography = HtmlSanitizerHelper.SanitizeHtml(authorDto.Biography);

        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            throw new ArgumentException("Author not found");
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
    }

    public async Task<AuthorRelationshipDto> AddAuthorRelationshipAsync(CreateAuthorRelationshipDto relationshipDto)
    {
        // Check if both authors exist
        var fromAuthor = await _context.Authors.FindAsync(relationshipDto.FromAuthorId);
        var toAuthor = await _context.Authors.FindAsync(relationshipDto.ToAuthorId);

        if (fromAuthor == null || toAuthor == null)
        {
            throw new ArgumentException("One or both authors not found");
        }

        // Check if relationship already exists
        var existingRelationship = await _context.AuthorRelationships
            .FirstOrDefaultAsync(r => r.FromAuthorId == relationshipDto.FromAuthorId &&
                                     r.ToAuthorId == relationshipDto.ToAuthorId);

        if (existingRelationship != null)
        {
            throw new ArgumentException("Relationship already exists");
        }

        var relationship = new AuthorRelationship
        {
            FromAuthorId = relationshipDto.FromAuthorId,
            ToAuthorId = relationshipDto.ToAuthorId,
            RelationshipType = relationshipDto.RelationshipType
        };

        _context.AuthorRelationships.Add(relationship);
        await _context.SaveChangesAsync();

        // Return the created relationship with names
        var createdRelationship = await _context.AuthorRelationships
            .Include(r => r.FromAuthor)
            .Include(r => r.ToAuthor)
            .FirstOrDefaultAsync(r => r.Id == relationship.Id);

        if (createdRelationship == null)
        {
            throw new InvalidOperationException("Created relationship not found.");
        }

        return new AuthorRelationshipDto
        {
            Id = createdRelationship.Id,
            FromAuthorId = createdRelationship.FromAuthorId,
            FromAuthorName = createdRelationship.FromAuthor.Name,
            ToAuthorId = createdRelationship.ToAuthorId,
            ToAuthorName = createdRelationship.ToAuthor.Name,
            RelationshipType = createdRelationship.RelationshipType,
            RelationshipTypeDisplayName = createdRelationship.RelationshipType.GetDisplayName()
        };
    }

    public async Task RemoveAuthorRelationshipAsync(int relationshipId)
    {
        var relationship = await _context.AuthorRelationships.FindAsync(relationshipId);
        if (relationship == null)
        {
            throw new ArgumentException("Relationship not found");
        }

        _context.AuthorRelationships.Remove(relationship);
        await _context.SaveChangesAsync();
    }
}