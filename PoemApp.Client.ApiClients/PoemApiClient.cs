using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace PoemApp.Client.ApiClients;

public class PoemApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PoemApiClient> _logger;

    public PoemApiClient(IHttpClientFactory factory, ILogger<PoemApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<List<PoemDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<PoemDto>>("api/poems") ?? new List<PoemDto>();
    }

    public async Task<PagedResult<PoemDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 12, string? search = null, string? dynasty = null, string? category = null)
    {
        var url = $"api/poems/paged?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(dynasty)) url += $"&dynasty={Uri.EscapeDataString(dynasty)}";
        if (!string.IsNullOrWhiteSpace(category)) url += $"&category={Uri.EscapeDataString(category)}";

        var resp = await _http.GetFromJsonAsync<PagedResult<PoemDto>>(url);
        return resp ?? new PagedResult<PoemDto>();
    }

    public async Task<PoemDto?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<PoemDto>($"api/poems/{id}");
    }

    public async Task<List<PoemDto>> GetByAuthorAsync(int authorId)
    {
        return await _http.GetFromJsonAsync<List<PoemDto>>($"api/poems/author/{authorId}") ?? new List<PoemDto>();
    }

    public async Task<PoemDto?> CreateAsync(CreatePoemDto dto)
    {
        var resp = await _http.PostAsJsonAsync("api/poems", dto);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<PoemDto>();
    }

    public async Task<bool> UpdateAsync(int id, UpdatePoemDto dto)
    {
        var resp = await _http.PutAsJsonAsync($"api/poems/{id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/poems/{id}");
        return resp.IsSuccessStatusCode;
    }
}
