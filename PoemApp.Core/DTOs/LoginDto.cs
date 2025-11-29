// LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class LoginDto
{
    // 支持多种登录方式
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WeChatCode { get; set; }  // 微信登录code
    public string? QQCode { get; set; }     // QQ登录code
    public string? Phone { get; set; }      // 手机号登录
    public string? VerificationCode { get; set; } // 验证码
}