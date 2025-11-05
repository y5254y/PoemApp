using System.Net.Http.Json;
using System.Security.Claims;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class AdminAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly ITokenService _tokenService;

    public AdminAuthService(IHttpClientFactory factory, ITokenService tokenService)
    {
        _factory = factory;
        _tokenService = tokenService;
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
                return err ?? new LoginResultDto { Success = false, Message = "登录失败" };
            }

            var result = await resp.Content.ReadFromJsonAsync<LoginResultDto>();
            if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
            {
                await _tokenService.SetTokenAsync(result.Token);
            }

            return result ?? new LoginResultDto { Success = false, Message = "登录失败" };
        }
        catch (HttpRequestException ex)
        {
            // 无法连接到 API（例如 API 未运行或端口不对）
            return new LoginResultDto { Success = false, Message = $"无法连接到后端 API：{ex.Message}. 请确保 PoemApp.API 已启动并且 Api:BaseUrl 配置正确。" };
        }
        catch (Exception ex)
        {
            return new LoginResultDto { Success = false, Message = $"登录时发生错误：{ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
        await Task.CompletedTask;
    }
}
