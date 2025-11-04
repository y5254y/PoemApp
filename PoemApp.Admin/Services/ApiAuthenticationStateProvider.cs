using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _factory;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public ApiAuthenticationStateProvider(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var client = _factory.CreateClient("Api");

        // Try to read token from localStorage via JS interop could be done, but for simplicity check a /me endpoint
        try
        {
            var resp = await client.GetAsync("api/auth/me");
            if (!resp.IsSuccessStatusCode)
                return new AuthenticationState(_anonymous);

            var user = await resp.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }, "apiauth");

            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(UserDto user)
    {
        var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }, "apiauth");

        var principal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
