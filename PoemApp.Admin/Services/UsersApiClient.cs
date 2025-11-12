using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace PoemApp.Admin.Services;

public class UsersApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<UsersApiClient> _logger;

    public UsersApiClient(IHttpClientFactory factory, ILogger<UsersApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        _logger.LogInformation("UsersApiClient.GetAllAsync: calling GET api/users");
        var list = await _http.GetFromJsonAsync<List<UserDto>>("api/users") ?? new List<UserDto>();
        _logger.LogInformation("UsersApiClient.GetAllAsync: returned {Count} users", list.Count);
        return list;
    }

    public async Task<UserDetailDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("UsersApiClient.GetByIdAsync: GET api/users/{Id}", id);
        return await _http.GetFromJsonAsync<UserDetailDto>($"api/users/{id}");
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        _logger.LogInformation("UsersApiClient.CreateAsync: POST api/users");
        var resp = await _http.PostAsJsonAsync("api/users", dto);
        _logger.LogInformation("CreateAsync: response {Status}", resp.StatusCode);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        _logger.LogInformation("UsersApiClient.UpdateAsync: PUT api/users/{Id}", id);
        var resp = await _http.PutAsJsonAsync($"api/users/{id}", dto);
        _logger.LogInformation("UpdateAsync: response {Status}", resp.StatusCode);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("UsersApiClient.DeleteAsync: DELETE api/users/{Id}", id);
        var resp = await _http.DeleteAsync($"api/users/{id}");
        _logger.LogInformation("DeleteAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }
}
