using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IQuoteService
{
    Task<IEnumerable<QuoteDto>> GetAllQuotesAsync();
    Task<QuoteDto?> GetQuoteByIdAsync(int id);
    Task<QuoteDto> AddQuoteAsync(CreateQuoteDto dto);
    Task UpdateQuoteAsync(int id, UpdateQuoteDto dto);
    Task DeleteQuoteAsync(int id);

    Task<IEnumerable<QuoteDto>> GetQuotesByAuthorAsync(int authorId);
    Task<IEnumerable<QuoteDto>> GetQuotesByPoemAsync(int poemId);
}