using System.Net.Http.Json;
using PoemApp.Core.DTOs;

namespace PoemApp.Client.ApiClients;

public class QuotesApiClient
{
    private readonly HttpClient _http;

    public QuotesApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<List<QuoteDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<QuoteDto>>("api/quotes") ?? new List<QuoteDto>();
    }

    public async Task<QuoteDto?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<QuoteDto>($"api/quotes/{id}");
    }

    public async Task<List<QuoteDto>> GetByAuthorAsync(int authorId)
    {
        return await _http.GetFromJsonAsync<List<QuoteDto>>($"api/quotes/byauthor/{authorId}") ?? new List<QuoteDto>();
    }

    public async Task<List<QuoteDto>> GetByPoemAsync(int poemId)
    {
        return await _http.GetFromJsonAsync<List<QuoteDto>>($"api/quotes/bypoem/{poemId}") ?? new List<QuoteDto>();
    }

    public async Task<QuoteDto?> CreateAsync(CreateQuoteDto dto)
    {
        var resp = await _http.PostAsJsonAsync("api/quotes", dto);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<QuoteDto>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuoteDto dto)
    {
        var resp = await _http.PutAsJsonAsync($"api/quotes/{id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/quotes/{id}");
        return resp.IsSuccessStatusCode;
    }
}
