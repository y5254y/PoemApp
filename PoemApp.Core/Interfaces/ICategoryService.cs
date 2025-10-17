using PoemApp.Core.DTOs;
using PoemApp.Core.Enums;

namespace PoemApp.Core.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto?> GetCategoryByTypeAsync(CategoryTypeEnum type);
    Task<CategoryDto> AddCategoryAsync(CreateCategoryDto categoryDto);
    Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task DeleteCategoryAsync(int id);
    Task AddPoemToCategoryAsync(int poemId, int categoryId);
    Task RemovePoemFromCategoryAsync(int poemId, int categoryId);
}