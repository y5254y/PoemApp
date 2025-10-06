using Microsoft.EntityFrameworkCore;
using PoemApp.API.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Services
{
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
                    Background = p.Background,
                    Translation = p.Translation,
                    Annotation = p.Annotation,
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
                Background = poem.Background,
                Translation = poem.Translation,
                Annotation = poem.Annotation,
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
                    Background = p.Background,
                    Translation = p.Translation,
                    Annotation = p.Annotation,
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
                    Background = p.Background,
                    Translation = p.Translation,
                    Annotation = p.Annotation,
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
    }
}