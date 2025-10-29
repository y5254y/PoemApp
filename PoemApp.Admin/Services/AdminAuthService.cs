using PoemApp.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace PoemApp.Admin.Services
{
    public interface IAdminAuthService
    {
        Task<LoginResultDto?> LoginAsync(string username, string password);
        Task LogoutAsync();
        bool IsAuthenticated();
        bool IsAdmin();
        string? GetUserName();
        string? GetUserRole();
    }

    public class AdminAuthService : IAdminAuthService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AdminAuthService> _logger;

        public AdminAuthService(IApiService apiService, ILogger<AdminAuthService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<LoginResultDto?> LoginAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("开始用户登录: {Username}", username);

                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };

                var result = await _apiService.PostAsync<LoginResultDto>("auth/login", loginDto);

                if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
                {
                    _apiService.SetToken(result.Token);
                    _logger.LogInformation("用户登录成功: {Username}", username);
                    return result;
                }
                else
                {
                    _logger.LogWarning("用户登录失败: {Username}, 原因: {Message}", username, result?.Message);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户登录过程中发生异常: {Username}", username);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            _apiService.SetToken(null);
            _logger.LogInformation("用户已注销");
            await Task.CompletedTask;
        }

        public bool IsAuthenticated()
        {
            // 简化实现，实际应该检查认证状态
            return !string.IsNullOrEmpty(_apiService.GetToken());
        }

        public bool IsAdmin()
        {
            // 简化实现
            return true;
        }

        public string? GetUserName()
        {
            return "管理员";
        }

        public string? GetUserRole()
        {
            return "Admin";
        }
    }
}