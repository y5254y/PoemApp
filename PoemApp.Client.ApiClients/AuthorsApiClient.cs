using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;
using PoemApp.Core.Enums;

namespace PoemApp.Client.ApiClients;

public class AuthorsApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthorsApiClient> _logger;

    public AuthorsApiClient(IHttpClientFactory factory, ILogger<AuthorsApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<PagedResult<AuthorDto>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        _logger.LogInformation("AuthorsApiClient.GetAllAsync: calling GET api/authors");
        var query = new List<string>();
        if (page > 1) query.Add($"page={page}");
        if (pageSize != 20) query.Add($"pageSize={pageSize}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        var url = "api/authors" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);
        var result = await _http.GetFromJsonAsync<PagedResult<AuthorDto>>(url);
        _logger.LogInformation("AuthorsApiClient.GetAllAsync: returned {Count} authors (total={Total})", result?.Items?.Count ?? 0, result?.TotalCount ?? 0);
        return result ?? new PagedResult<AuthorDto>();
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

    public async Task<AuthorRelationshipDto?> AddRelationshipAsync(CreateAuthorRelationshipDto dto)
    {
        _logger.LogInformation("AuthorsApiClient.AddRelationshipAsync: POST api/authors/relationships");
        var resp = await _http.PostAsJsonAsync("api/authors/relationships", dto);
        _logger.LogInformation("AddRelationshipAsync: response {Status}", resp.StatusCode);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthorRelationshipDto>();
    }

    public async Task<bool> RemoveRelationshipAsync(int id)
    {
        _logger.LogInformation("AuthorsApiClient.RemoveRelationshipAsync: DELETE api/authors/relationships/{Id}", id);
        var resp = await _http.DeleteAsync($"api/authors/relationships/{id}");
        _logger.LogInformation("RemoveRelationshipAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }
}
