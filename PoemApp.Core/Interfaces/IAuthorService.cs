using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
    Task<AuthorDto> GetAuthorByIdAsync(int id);
    Task<AuthorDto> AddAuthorAsync(CreateAuthorDto authorDto);
    Task UpdateAuthorAsync(int id, UpdateAuthorDto authorDto);
    Task DeleteAuthorAsync(int id);
    Task<AuthorRelationshipDto> AddAuthorRelationshipAsync(CreateAuthorRelationshipDto relationshipDto);
    Task RemoveAuthorRelationshipAsync(int relationshipId);
}