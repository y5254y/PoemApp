using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PoemApp.Core.DTOs;
using System.Security.Claims;

namespace PoemApp.Admin.Services;

public class AuthService
{
    private readonly IApiService _apiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IApiService apiService, IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResultDto?> LoginAsync(string username, string password)
    {
        var loginDto = new LoginDto
        {
            Username = username,
            Password = password
        };

        var result = await _apiService.PostAsync<LoginResultDto>("auth/login", loginDto);

        if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
        {
            _apiService.SetToken(result.Token);

            // 设置认证Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.User?.Username ?? ""),
                new Claim(ClaimTypes.NameIdentifier, result.User?.Id.ToString() ?? ""),
                new Claim(ClaimTypes.Role, result.User?.Role.ToString() ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            if (_httpContextAccessor.HttpContext != null)
            {
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    });
            }

            return result;
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        _apiService.SetToken(null);

        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public bool IsAdmin()
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
    }

    public string? GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "用户";
    }

    public string? GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "User";
    }
}