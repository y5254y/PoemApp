using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using PoemApp.Core.Enums;
using Microsoft.Extensions.Logging;

namespace PoemApp.Client.ApiClients;

public class CategoriesApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<CategoriesApiClient> _logger;

    public CategoriesApiClient(IHttpClientFactory factory, ILogger<CategoriesApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        _logger.LogInformation("CategoriesApiClient.GetAllAsync: calling GET api/categories");
        var list = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? new List<CategoryDto>();
        _logger.LogInformation("CategoriesApiClient.GetAllAsync: returned {Count} categories", list.Count);
        return list;
    }

    public async Task<PagedResult<CategoryDto>> GetPagedAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var q = System.Web.HttpUtility.ParseQueryString(string.Empty);
        q["page"] = page.ToString();
        q["pageSize"] = pageSize.ToString();
        if (!string.IsNullOrWhiteSpace(search)) q["search"] = search;
        var url = "api/categories/paged?" + q.ToString();
        _logger.LogInformation("CategoriesApiClient.GetPagedAsync: GET {Url}", url);
        var res = await _http.GetFromJsonAsync<PagedResult<CategoryDto>>(url) ?? new PagedResult<CategoryDto>();
        _logger.LogInformation("CategoriesApiClient.GetPagedAsync: returned {Count}/{Total}", res.Items.Count, res.TotalCount);
        return res;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("CategoriesApiClient.GetByIdAsync: GET api/categories/{Id}", id);
        try
        {
            var dto = await _http.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
            _logger.LogInformation("CategoriesApiClient.GetByIdAsync: got {Found}", dto != null);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed for id {Id}", id);
            throw;
        }
    }

    public async Task<CategoryDto?> CreateAsync(CreateCategoryDto dto)
    {
        _logger.LogInformation("CategoriesApiClient.CreateAsync: POST api/categories");
        var resp = await _http.PostAsJsonAsync("api/categories", dto);
        _logger.LogInformation("CreateAsync: response {Status}", resp.StatusCode);
        if (!resp.IsSuccessStatusCode) return null;
        var created = await resp.Content.ReadFromJsonAsync<CategoryDto>();
        _logger.LogInformation("CreateAsync: created id {Id}", created?.Id);
        return created;
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        _logger.LogInformation("CategoriesApiClient.UpdateAsync: PUT api/categories/{Id}", id);
        var resp = await _http.PutAsJsonAsync($"api/categories/{id}", dto);
        _logger.LogInformation("UpdateAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("CategoriesApiClient.DeleteAsync: DELETE api/categories/{Id}", id);
        var resp = await _http.DeleteAsync($"api/categories/{id}");
        _logger.LogInformation("DeleteAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> AddPoemAsync(int categoryId, int poemId)
    {
        _logger.LogInformation("CategoriesApiClient.AddPoemAsync: POST api/categories/{CategoryId}/poems/{PoemId}", categoryId, poemId);
        var resp = await _http.PostAsync($"api/categories/{categoryId}/poems/{poemId}", null);
        _logger.LogInformation("AddPoemAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> RemovePoemAsync(int categoryId, int poemId)
    {
        _logger.LogInformation("CategoriesApiClient.RemovePoemAsync: DELETE api/categories/{CategoryId}/poems/{PoemId}", categoryId, poemId);
        var resp = await _http.DeleteAsync($"api/categories/{categoryId}/poems/{poemId}");
        _logger.LogInformation("RemovePoemAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }
}
