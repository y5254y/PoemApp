using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace PoemApp.Admin.Services;

public class AuthorsApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthorsApiClient> _logger;

    public AuthorsApiClient(IHttpClientFactory factory, ILogger<AuthorsApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        _logger.LogInformation("AuthorsApiClient.GetAllAsync: calling GET api/authors");
        var list = await _http.GetFromJsonAsync<List<AuthorDto>>("api/authors") ?? new List<AuthorDto>();
        _logger.LogInformation("AuthorsApiClient.GetAllAsync: returned {Count} authors", list.Count);
        return list;
    }

    public async Task<AuthorDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("AuthorsApiClient.GetByIdAsync: GET api/authors/{Id}", id);
        return await _http.GetFromJsonAsync<AuthorDto>($"api/authors/{id}");
    }

    public async Task<AuthorDto?> CreateAsync(CreateAuthorDto dto)
    {
        _logger.LogInformation("AuthorsApiClient.CreateAsync: POST api/authors");
        var resp = await _http.PostAsJsonAsync("api/authors", dto);
        _logger.LogInformation("CreateAsync: response {Status}", resp.StatusCode);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthorDto>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAuthorDto dto)
    {
        _logger.LogInformation("AuthorsApiClient.UpdateAsync: PUT api/authors/{Id}", id);
        var resp = await _http.PutAsJsonAsync($"api/authors/{id}", dto);
        _logger.LogInformation("UpdateAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("AuthorsApiClient.DeleteAsync: DELETE api/authors/{Id}", id);
        var resp = await _http.DeleteAsync($"api/authors/{id}");
        _logger.LogInformation("DeleteAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }
}
