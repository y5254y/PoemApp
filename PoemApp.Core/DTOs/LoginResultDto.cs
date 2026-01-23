// LoginResultDto.cs
namespace PoemApp.Core.DTOs;

public class LoginResultDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public BasicUserDto? User { get; set; }
    public string? Message { get; set; }
}