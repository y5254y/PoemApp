using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;
using PoemApp.Core.Interfaces;
namespace PoemApp.Admin.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _factory;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ApiAuthenticationStateProvider> _logger;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public ApiAuthenticationStateProvider(IHttpClientFactory factory, ITokenService tokenService, ILogger<ApiAuthenticationStateProvider> logger)
    {
        _factory = factory;
        _tokenService = tokenService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var client = _factory.CreateClient("Api");

        try
        {
            _logger.LogDebug("GetAuthenticationStateAsync: attempting to read token from token service");
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("GetAuthenticationStateAsync: no token found");
                return new AuthenticationState(_anonymous);
            }

            _logger.LogDebug("GetAuthenticationStateAsync: token found, calling /api/auth/me to validate");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("api/auth/me");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetAuthenticationStateAsync: /api/auth/me returned {StatusCode}", resp.StatusCode);
                return new AuthenticationState(_anonymous);
            }

            var user = await resp.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
            {
                _logger.LogWarning("GetAuthenticationStateAsync: /api/auth/me returned success but user payload was null");
                return new AuthenticationState(_anonymous);
            }

            _logger.LogInformation("GetAuthenticationStateAsync: authenticated as {Username}", user.Username);

            // 在所有 user.Username 访问前添加 null 检查，避免空引用异常
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }, "apiauth");

            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthenticationStateAsync: exception while validating token");
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task NotifyUserAuthenticationAsync(UserDto user, string token)
    {
        try
        {
            _logger.LogInformation("NotifyUserAuthenticationAsync: setting token for user {Username}", user?.Username);
            if (!string.IsNullOrEmpty(token))
            {
                await _tokenService.SetTokenAsync(token);
                _logger.LogDebug("NotifyUserAuthenticationAsync: token stored");
            }

            // 在所有 user.Username 访问前添加 null 检查，避免空引用异常
            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }, "apiauth");

            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotifyUserAuthenticationAsync: error while storing token or notifying state");
        }
    }

    public async Task NotifyUserLogoutAsync()
    {
        try
        {
            _logger.LogInformation("NotifyUserLogoutAsync: removing token and notifying logout");
            await _tokenService.RemoveTokenAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotifyUserLogoutAsync: error while removing token");
        }
    }
}
