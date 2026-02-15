using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IPoemService
{
    Task<IEnumerable<PoemDto>> GetAllPoemsAsync();
    Task<PagedResult<PoemDto>> GetPoemsPagedAsync(int pageNumber, int pageSize, string? search = null, string? dynasty = null);
    Task<PoemDto> GetPoemByIdAsync(int id);
    Task<IEnumerable<PoemDto>> GetPoemsByCategoryAsync(string categoryName);
    Task<IEnumerable<PoemDto>> GetPoemsByAuthorAsync(int authorId);
    Task<PoemDto> AddPoemAsync(CreatePoemDto poemDto);
    Task UpdatePoemAsync(int id, UpdatePoemDto poemDto);
    Task DeletePoemAsync(int id);

    Task AddCategoryToPoemAsync(int poemId, int categoryId);
    Task RemoveCategoryFromPoemAsync(int poemId, int categoryId);
}