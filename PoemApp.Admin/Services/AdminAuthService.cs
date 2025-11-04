using System.Net.Http.Json;
using System.Security.Claims;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class AdminAuthService
{
    private readonly IHttpClientFactory _factory;

    public AdminAuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var client = _factory.CreateClient("Api");
        var resp = await client.PostAsJsonAsync("api/auth/login", dto);
        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadFromJsonAsync<LoginResultDto>();
            return err ?? new LoginResultDto { Success = false, Message = "µÇÂ¼Ê§°Ü" };
        }

        var result = await resp.Content.ReadFromJsonAsync<LoginResultDto>();
        return result ?? new LoginResultDto { Success = false, Message = "µÇÂ¼Ê§°Ü" };
    }

    public async Task LogoutAsync()
    {
        // No server-side logout for token-based; client will wipe token
        await Task.CompletedTask;
    }
}
