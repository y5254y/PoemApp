using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _factory;
    private readonly ITokenService _tokenService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public ApiAuthenticationStateProvider(IHttpClientFactory factory, ITokenService tokenService)
    {
        _factory = factory;
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var client = _factory.CreateClient("Api");

        try
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(_anonymous);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("api/auth/me");
            if (!resp.IsSuccessStatusCode)
                return new AuthenticationState(_anonymous);

            var user = await resp.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }, "apiauth");

            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task NotifyUserAuthenticationAsync(UserDto user, string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            await _tokenService.SetTokenAsync(token);
        }

        var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }, "apiauth");

        var principal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task NotifyUserLogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
