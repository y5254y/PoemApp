using System.Net.Http.Json;
using PoemApp.Core.DTOs;

namespace PoemApp.Client.ApiClients;

public class PoemApiClient
{
    private readonly HttpClient _http;

    public PoemApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<List<PoemDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<PoemDto>>("api/poems") ?? new List<PoemDto>();
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
