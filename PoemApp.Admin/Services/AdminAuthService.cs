using System.Net.Http.Json;
using System.Security.Claims;
using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace PoemApp.Admin.Services;

public class AdminAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AdminAuthService> _logger;
    private readonly IJSRuntime _js;

    public AdminAuthService(IHttpClientFactory factory, ITokenService tokenService, ILogger<AdminAuthService> logger, IJSRuntime js)
    {
        _factory = factory;
        _tokenService = tokenService;
        _logger = logger;
        _js = js;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var client = _factory.CreateClient("Api");
        try
        {
            var resp = await client.PostAsJsonAsync("api/auth/login", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadFromJsonAsync<LoginResultDto>();
                _logger.LogWarning("LoginAsync: login failed, status {Status}", resp.StatusCode);
                return err ?? new LoginResultDto { Success = false, Message = "登录失败" };
            }

            var result = await resp.Content.ReadFromJsonAsync<LoginResultDto>();
            if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
            {
                // store token via token service
                await _tokenService.SetTokenAsync(result.Token);
                _logger.LogInformation("LoginAsync: token stored via ITokenService for user {User}", result.User?.Username);

                // For debugging: also write token to browser localStorage under a debug key so you can inspect easily
                try
                {
                    await _js.InvokeVoidAsync("localStorage.setItem", "poemapp_token_debug", result.Token);
                    _logger.LogDebug("LoginAsync: token also written to localStorage 'poemapp_token_debug' for debugging");
                }
                catch (Exception jsEx)
                {
                    _logger.LogWarning(jsEx, "LoginAsync: failed to write debug token to localStorage");
                }
            }

            return result ?? new LoginResultDto { Success = false, Message = "登录失败" };
        }
        catch (HttpRequestException ex)
        {
            // 无法连接到 API（例如 API 未运行或端口不对）
            _logger.LogError(ex, "LoginAsync: HttpRequestException when calling API");
            return new LoginResultDto { Success = false, Message = $"无法连接到后端 API：{ex.Message}. 请确保 PoemApp.API 已启动并且 Api:BaseUrl 配置正确。" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoginAsync: unexpected exception");
            return new LoginResultDto { Success = false, Message = $"登录时发生错误：{ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _tokenService.RemoveTokenAsync();
            // remove debug localStorage key
            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", "poemapp_token_debug");
            }
            catch { }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogoutAsync: error removing token");
        }
    }
}
