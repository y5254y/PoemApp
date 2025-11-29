namespace PoemApp.Core.Interfaces;

public interface ITokenService
{
    Task SetTokenAsync(string? token);
    Task<string?> GetTokenAsync();
    Task RemoveTokenAsync();
}
