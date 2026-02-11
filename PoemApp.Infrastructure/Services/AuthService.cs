using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PoemApp.Infrastructure.Data;
using PoemApp.Core.DTOs;
using PoemApp.Core.Entities;
using PoemApp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using PoemApp.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace PoemApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IAppLogger _logger;
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IWeChatLoginService _weChatLoginService;

    public AuthService(AppDbContext context, IConfiguration configuration, IUserService userService, IAppLogger logger, System.Net.Http.HttpClient httpClient, IWeChatLoginService weChatLoginService)
    {
        _context = context;
        _configuration = configuration;
        _userService = userService;
        _logger = logger;
        _httpClient = httpClient;
        _weChatLoginService = weChatLoginService;
        _logger.LogInformation("AuthService 实例化完成");
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation($"用户登录尝试: {loginDto.Username}");
        // 根据提供的登录方式处理
        if (!string.IsNullOrEmpty(loginDto.WeChatCode))
        {
            return await WeChatLoginAsync(loginDto.WeChatCode);
        }
        else if (!string.IsNullOrEmpty(loginDto.QQCode))
        {
            return await QQLoginAsync(loginDto.QQCode);
        }
        else if (!string.IsNullOrEmpty(loginDto.Phone) && !string.IsNullOrEmpty(loginDto.VerificationCode))
        {
            return await PhoneLoginAsync(loginDto.Phone, loginDto.VerificationCode);
        }
        else if (!string.IsNullOrEmpty(loginDto.Username) && !string.IsNullOrEmpty(loginDto.Password))
        {
            return await UsernamePasswordLoginAsync(loginDto.Username, loginDto.Password);
        }

        return new LoginResultDto { Success = false, Message = "不支持的登录方式" };
    }

    public async Task<LoginResultDto> WeChatLoginAsync(string code)
    {
        try
        {
            var weChatInfo = await GetWeChatOpenIdAsync(code);

            if (weChatInfo == null)
            {
                return new LoginResultDto { Success = false, Message = "微信登录失败" };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.WeChatId == weChatInfo.OpenId);

            if (user == null)
            {
                var createUserDto = new CreateUserDto
                {
                    Username = weChatInfo.Nickname ?? $"微信用户_{DateTime.Now.Ticks}",
                    WeChatId = weChatInfo.OpenId
                };

                var newUser = await _userService.CreateUserAsync(createUserDto);
                user = await _context.Users.FindAsync(newUser.Id);
            }

            if (user == null)
            {
                return new LoginResultDto { Success = false, Message = "用户创建失败" };
            }

            var token = GenerateJwtToken(user);
            var userDto = new BasicUserDto
            {
                Id = user.Id,
                Username = user.Username,
                WeChatId = user.WeChatId,
                QQId = user.QQId,
                Phone = user.Phone,
                Points = user.Points,
                Role = user.Role.ToString(),
                RoleDisplayName = user.Role.GetDisplayName()
            };

            return new LoginResultDto
            {
                Success = true,
                Token = token,
                User = userDto,
                Message = "登录成功"
            };
        }
        catch (Exception ex)
        {
            return new LoginResultDto { Success = false, Message = $"登录失败: {ex.Message}" };
        }
    }

    // 修复 QQLoginAsync 方法中的类型转换问题
    public async Task<LoginResultDto> QQLoginAsync(string code)
    {
        try
        {
            var qqInfo = await GetQQOpenIdAsync(code);

            if (qqInfo == null)
            {
                return new LoginResultDto { Success = false, Message = "QQ登录失败" };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.QQId == qqInfo.OpenId);

            if (user == null)
            {
                var createUserDto = new CreateUserDto
                {
                    Username = qqInfo.Nickname ?? $"QQ用户_{DateTime.Now.Ticks}",
                    QQId = qqInfo.OpenId
                };

                var newUser = await _userService.CreateUserAsync(createUserDto);
                user = await _context.Users.FindAsync(newUser.Id);
            }

            if (user == null)
            {
                return new LoginResultDto { Success = false, Message = "用户创建失败" };
            }

            var token = GenerateJwtToken(user);
            var userDto = new BasicUserDto
            {
                Id = user.Id,
                Username = user.Username,
                WeChatId = user.WeChatId,
                QQId = user.QQId,
                Phone = user.Phone,
                Points = user.Points,
                Role = user.Role.ToString(),
                RoleDisplayName = user.Role.GetDisplayName()
            };

            return new LoginResultDto
            {
                Success = true,
                Token = token,
                User = userDto,
                Message = "登录成功"
            };
        }
        catch (Exception ex)
        {
            return new LoginResultDto { Success = false, Message = $"登录失败: {ex.Message}" };
        }
    }

    public async Task<LoginResultDto> PhoneLoginAsync(string phone, string verificationCode)
    {
        // 验证验证码（需要实现短信服务）
        // 这里简化实现，实际应用中需要调用短信验证服务
        if (verificationCode != "123456") // 示例验证码
        {
            return new LoginResultDto { Success = false, Message = "验证码错误" };
        }

        // 查找或创建用户
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Phone == phone);

        if (user == null)
        {
            // 创建新用户
            var createUserDto = new CreateUserDto
            {
                Username = $"手机用户_{phone.Substring(7)}", // 取手机号后4位
                Phone = phone
            };

            var newUser = await _userService.CreateUserAsync(createUserDto);
            user = await _context.Users.FindAsync(newUser.Id);
        }

        // 新增非空检查
        if (user == null)
        {
            return new LoginResultDto { Success = false, Message = "用户创建失败" };
        }

        var token = GenerateJwtToken(user);
        var userDto = MapToBasicUserDto(user);

        return new LoginResultDto
        {
            Success = true,
            Token = token,
            User = userDto,
            Message = "登录成功"
        };
    }

    private async Task<LoginResultDto> UsernamePasswordLoginAsync(string username, string password)
    {
        // 首先需要确保用户实体有密码字段
        // 如果您的User实体没有PasswordHash和PasswordSalt字段，这部分需要修改

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        // 检查用户是否存在且密码正确
        if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            return new LoginResultDto { Success = false, Message = "用户名或密码错误" };
        }

        var token = GenerateJwtToken(user);
        var userDto = MapToBasicUserDto(user);

        return new LoginResultDto
        {
            Success = true,
            Token = token,
            User = userDto,
            Message = "登录成功"
        };
    }

    public async Task<LoginResultDto> RegisterAsync(RegisterDto registerDto)
    {
        // 检查用户名是否已存在
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

        if (existingUser != null)
        {
            return new LoginResultDto { Success = false, Message = "用户名已存在" };
        }

        User user;

        if (!string.IsNullOrEmpty(registerDto.WeChatCode))
        {
            // 微信注册
            var weChatInfo = await GetWeChatOpenIdAsync(registerDto.WeChatCode);
            user = new User
            {
                Username = registerDto.Username,
                WeChatId = weChatInfo.OpenId, // 使用 WeChatId
                CreatedAt = DateTime.UtcNow
            };
        }
        else if (!string.IsNullOrEmpty(registerDto.QQCode))
        {
            // QQ注册
            var qqInfo = await GetQQOpenIdAsync(registerDto.QQCode);
            user = new User
            {
                Username = registerDto.Username,
                QQId = qqInfo.OpenId, // 使用 QQId
                CreatedAt = DateTime.UtcNow
            };
        }
        else if (!string.IsNullOrEmpty(registerDto.Phone))
        {
            // 手机号注册
            user = new User
            {
                Username = registerDto.Username,
                Phone = registerDto.Phone,
                CreatedAt = DateTime.UtcNow
            };
        }
        else if (!string.IsNullOrEmpty(registerDto.Password))
        {
            // 密码注册
            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow
            };
        }
        else
        {
            return new LoginResultDto { Success = false, Message = "请提供密码或第三方登录凭证" };
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var userDto = MapToBasicUserDto(user);

        return new LoginResultDto
        {
            Success = true,
            Token = token,
            User = userDto,
            Message = "注册成功"
        };
    }

    private BasicUserDto MapToBasicUserDto(User user)
    {
        return new BasicUserDto
        {
            Id = user.Id,
            Username = user.Username,
            WeChatId = user.WeChatId,
            QQId = user.QQId,
            Phone = user.Phone,
            Points = user.Points,
            Role = user.Role.ToString(),
            RoleDisplayName = user.Role.GetDisplayName()
        };
    }

    private string GenerateJwtToken(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "用户对象不能为 null");

        try
        {
            // 确保密钥长度足够（至少 64 字节/512 位）
            var jwtKey = _configuration["Jwt:Key"];

            // 如果配置的密钥长度不足，使用一个安全的默认密钥
            if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 64)
            {
                // 使用一个足够长的默认密钥（开发环境）
                jwtKey = "ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatIsAtLeast64BytesLongToMeetHMACSHA512Requirements";
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            // 如果 JWT 生成失败，返回一个简单的 token（仅用于开发）
            Console.WriteLine($"JWT Token 生成失败: {ex.Message}");
            return $"dev-token-{user.Id}-{Guid.NewGuid()}";
        }
    }
    // 生成随机密钥的辅助方法
    private string GenerateRandomKey(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

        // 微信API调用（改为真实实现：调用 WeChatLoginService）
    private async Task<WeChatUserInfo> GetWeChatOpenIdAsync(string code)
    {
            try
            {
                var resp = await _weChatLoginService.Code2SessionAsync(code);
            if (resp == null)
            {
                _logger.LogWarning("WeChat code2session 返回 null");
                return null!;
            }

            if (resp.Errcode.HasValue && resp.Errcode.Value != 0)
            {
                _logger.LogWarning($"WeChat 接口错误: {resp.Errmsg} ({resp.Errcode})");
                return null!;
            }

            return new WeChatUserInfo
            {
                OpenId = resp.OpenId ?? string.Empty,
                Nickname = string.Empty,
                HeadImgUrl = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"GetWeChatOpenIdAsync 异常: {ex.Message}", ex);
            return null!;
        }
    }

    // QQAPI调用（简化实现）
    private async Task<QQUserInfo> GetQQOpenIdAsync(string code)
    {
        // 简化实现，返回模拟数据，实际应调用 QQ 的授权/接口
        return await Task.FromResult(new QQUserInfo
        {
            OpenId = $"qq_openid_{code}",
            Nickname = "QQ用户",
            HeadImgUrl = "https://example.com/avatar.jpg"
        });
    }

    // 微信 code2session 响应 DTO
    private class WeChatCode2SessionResponse
    {
        public string? openid { get; set; }
        public string? session_key { get; set; }
        public string? unionid { get; set; }
        public string? errcode { get; set; }
        public string? errmsg { get; set; }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"ChangePasswordAsync: user {userId} not found");
            return false;
        }

        // Verify current password
        if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning($"ChangePasswordAsync: current password mismatch for user {userId}");
            return false;
        }

        // Create new password hash
        CreatePasswordHash(newPassword, out byte[] newHash, out byte[] newSalt);
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"ChangePasswordAsync: password changed for user {userId}");
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        // Support development fallback tokens that start with "dev-token-<userId>-<guid>"
        if (token.StartsWith("dev-token-"))
        {
            var parts = token.Split('-', 3);
            if (parts.Length >= 2 && int.TryParse(parts[1], out var devUserId))
            {
                var u = await _context.Users.FindAsync(devUserId);
                return u != null;
            }
            return false;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 16)
            {
                _logger.LogWarning("ValidateTokenAsync: JWT key missing or too short");
                return false;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = !string.IsNullOrEmpty(_configuration["Jwt:Issuer"]),
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = !string.IsNullOrEmpty(_configuration["Jwt:Audience"]),
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return validatedToken != null;
        }
        catch (Exception ex)
        {
            _logger.LogError("ValidateTokenAsync: token validation failed", ex);
            return false;
        }
    }

    public async Task<BasicUserDto?> GetUserFromTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        // Handle development fallback token
        if (token.StartsWith("dev-token-"))
        {
            var parts = token.Split('-', 3);
            if (parts.Length >= 2 && int.TryParse(parts[1], out var devUserId))
            {
                var userEntity = await _context.Users.FindAsync(devUserId);
                if (userEntity == null) return null;
                return new BasicUserDto
                {
                    Id = userEntity.Id,
                    Username = userEntity.Username,
                    WeChatId = userEntity.WeChatId,
                    QQId = userEntity.QQId,
                    Phone = userEntity.Phone,
                    Points = userEntity.Points,
                    Role = userEntity.Role.ToString(),
                    RoleDisplayName = userEntity.Role.GetDisplayName()
                };
            }
            return null;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 16)
            {
                _logger.LogWarning("GetUserFromTokenAsync: JWT key missing or too short");
                return null;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = !string.IsNullOrEmpty(_configuration["Jwt:Issuer"]),
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = !string.IsNullOrEmpty(_configuration["Jwt:Audience"]),
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            if (principal == null)
                return null;

            // Try common claim types for user id
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("id") ?? principal.FindFirst("sub");
            if (idClaim == null) return null;

            if (!int.TryParse(idClaim.Value, out var userId)) return null;

            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity == null) return null;

            return new BasicUserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                WeChatId = userEntity.WeChatId,
                QQId = userEntity.QQId,
                Phone = userEntity.Phone,
                Points = userEntity.Points,
                Role = userEntity.Role.ToString(),
                RoleDisplayName = userEntity.Role.GetDisplayName()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("GetUserFromTokenAsync: failed to validate token or retrieve user", ex);
            return null;
        }
    }

    // 微信用户信息DTO
    public class WeChatUserInfo
    {
        public string OpenId { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string HeadImgUrl { get; set; } = string.Empty;
    }

    // QQ用户信息DTO
    public class QQUserInfo
    {
        public string OpenId { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string HeadImgUrl { get; set; } = string.Empty;
    }
}