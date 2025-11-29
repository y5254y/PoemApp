using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;
using PoemApp.Core.Enums;

namespace PoemApp.Client.ApiClients;

public class UsersApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<UsersApiClient> _logger;

    public UsersApiClient(IHttpClientFactory factory, ILogger<UsersApiClient> logger)
    {
        _http = factory.CreateClient("Api");
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null, UserRole? role = null, bool? isVip = null)
    {
        var query = new List<string>();
        if (page > 1) query.Add($"page={page}");
        if (pageSize != 20) query.Add($"pageSize={pageSize}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (role.HasValue) query.Add($"role={role.Value}");
        if (isVip.HasValue) query.Add($"isVip={isVip.Value}");

        var url = "api/users" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);
        _logger.LogInformation("UsersApiClient.GetAllAsync: calling GET {Url}", url);
        var result = await _http.GetFromJsonAsync<PagedResult<UserDto>>(url);
        return result ?? new PagedResult<UserDto>();
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

    public async Task<bool> UpdateVipAsync(int userId, UpdateVipStatusDto dto)
    {
        _logger.LogInformation("UsersApiClient.UpdateVipAsync: PUT api/users/{UserId}/vip", userId);
        var resp = await _http.PutAsJsonAsync($"api/users/{userId}/vip", dto);
        _logger.LogInformation("UpdateVipAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveVipAsync(int userId)
    {
        _logger.LogInformation("UsersApiClient.RemoveVipAsync: DELETE api/users/{UserId}/vip", userId);
        var resp = await _http.DeleteAsync($"api/users/{userId}/vip");
        _logger.LogInformation("RemoveVipAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        _logger.LogInformation("UsersApiClient.ChangePasswordAsync: POST api/users/{UserId}/change-password", userId);
        var resp = await _http.PostAsJsonAsync($"api/users/{userId}/change-password", dto);
        _logger.LogInformation("ChangePasswordAsync: response {Status}", resp.StatusCode);
        return resp.IsSuccessStatusCode;
    }
}
