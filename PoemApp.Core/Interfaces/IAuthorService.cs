using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IAuthorService
{
    Task<PagedResult<AuthorDto>> GetAllAuthorsAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<AuthorDto> GetAuthorByIdAsync(int id);
    Task<AuthorDto> AddAuthorAsync(CreateAuthorDto authorDto);
    Task UpdateAuthorAsync(int id, UpdateAuthorDto authorDto);
    Task DeleteAuthorAsync(int id);
    Task<AuthorRelationshipDto> AddAuthorRelationshipAsync(CreateAuthorRelationshipDto relationshipDto);
    Task RemoveAuthorRelationshipAsync(int relationshipId);
}