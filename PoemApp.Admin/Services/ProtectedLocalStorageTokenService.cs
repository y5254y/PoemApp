using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace PoemApp.Admin.Services;

public class ProtectedLocalStorageTokenService : ITokenService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<ProtectedLocalStorageTokenService> _logger;
    private readonly IJSRuntime _js;
    private const string TokenKey = "poemapp_token";
    private const string DebugKey = "poemapp_token_debug";

    public ProtectedLocalStorageTokenService(ProtectedLocalStorage protectedLocalStorage, ILogger<ProtectedLocalStorageTokenService> logger, IJSRuntime js)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
        _js = js;
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            await _protectedLocalStorage.SetAsync(TokenKey, token);
            _logger.LogInformation("ProtectedLocalStorageTokenService: token stored (length={Length})", token?.Length ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProtectedLocalStorageTokenService: failed to set token");
            // fallback: attempt to write to plain localStorage for dev/debug
            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", DebugKey, token);
                _logger.LogWarning("ProtectedLocalStorageTokenService: fallback wrote token to localStorage debug key");
            }
            catch (Exception jsEx)
            {
                _logger.LogError(jsEx, "ProtectedLocalStorageTokenService: fallback write to localStorage failed");
            }
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>(TokenKey);
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _logger.LogInformation("ProtectedLocalStorageTokenService: token retrieved (length={Length})", result.Value?.Length ?? 0);
                return result.Value;
            }

            _logger.LogDebug("ProtectedLocalStorageTokenService: token not found in protected storage, attempting fallback to localStorage debug key");

            // Fallback: attempt to read from plain localStorage debug key (useful for debugging or when protected storage isn't available)
            try
            {
                var debugToken = await _js.InvokeAsync<string>("localStorage.getItem", DebugKey);
                if (!string.IsNullOrEmpty(debugToken))
                {
                    _logger.LogWarning("ProtectedLocalStorageTokenService: found debug token in localStorage (length={Length}), migrating to protected storage", debugToken.Length);
                    // Try to migrate into protected storage
                    try
                    {
                        await _protectedLocalStorage.SetAsync(TokenKey, debugToken);
                        _logger.LogInformation("ProtectedLocalStorageTokenService: migrated debug token into protected storage");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ProtectedLocalStorageTokenService: failed to migrate debug token into protected storage");
                    }

                    return debugToken;
                }
            }
            catch (Exception jsEx)
            {
                _logger.LogWarning(jsEx, "ProtectedLocalStorageTokenService: failed to read debug token from localStorage");
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProtectedLocalStorageTokenService: failed to get token from protected storage");
            // As a last resort, try reading debug key from localStorage
            try
            {
                var debugToken = await _js.InvokeAsync<string>("localStorage.getItem", DebugKey);
                if (!string.IsNullOrEmpty(debugToken))
                {
                    _logger.LogWarning("ProtectedLocalStorageTokenService: recovered debug token from localStorage after exception");
                    return debugToken;
                }
            }
            catch { }

            return null;
        }
    }

    public async Task RemoveTokenAsync()
    {
        try
        {
            await _protectedLocalStorage.DeleteAsync(TokenKey);
            _logger.LogInformation("ProtectedLocalStorageTokenService: token removed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProtectedLocalStorageTokenService: failed to remove token from protected storage");
        }

        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", DebugKey);
            _logger.LogInformation("ProtectedLocalStorageTokenService: debug token removed from localStorage");
        }
        catch { }
    }
}
