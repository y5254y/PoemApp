// IAuthService.cs
using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IAuthService
{
    // 登录相关
    Task<LoginResultDto> LoginAsync(LoginDto loginDto);
    Task<LoginResultDto> RegisterAsync(RegisterDto registerDto);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    // 第三方登录
    Task<LoginResultDto> WeChatLoginAsync(string code);
    Task<LoginResultDto> QQLoginAsync(string code);
    Task<LoginResultDto> PhoneLoginAsync(string phone, string verificationCode);

    // Token管理
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetUserFromTokenAsync(string token);
}