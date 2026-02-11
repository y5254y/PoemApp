// AuthController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login(LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResultDto>> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("wechat")]
    public async Task<ActionResult<LoginResultDto>> WeChatLogin([FromBody] WeChatLoginRequestDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.Code))
            return BadRequest(new { message = "缺少 code" });

        var result = await _authService.WeChatLoginAsync(request.Code);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // GET api/auth/me
    // Returns current authenticated user based on Bearer token
    [HttpGet("me")]
    public async Task<ActionResult<BasicUserDto>> Me()
    {
        // Look for Authorization header
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            return Unauthorized();

        var authHeader = authHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized();

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        // Ask the auth service to resolve user from token
        var user = await _authService.GetUserFromTokenAsync(token);
        if (user == null)
            return Unauthorized();

        return Ok(user);
    }
}