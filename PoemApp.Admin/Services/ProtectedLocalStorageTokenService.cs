using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PoemApp.Core.Interfaces;

namespace PoemApp.Admin.Services;

public class ProtectedLocalStorageTokenService : ITokenService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    //private readonly ILogger<ProtectedLocalStorageTokenService> _logger;
    private readonly IAppLogger _logger;
    private readonly IJSRuntime _js;
    private const string TokenKey = "poemapp_token";
    private const string DebugKey = "poemapp_token_debug";

    // In-memory cached token to avoid JS interop during server-side prerender or outside render cycle
    private string? _cachedToken;

    // Global static cache so background handlers (different DI scopes) can access the token in server process.
    // NOTE: This is acceptable here because this admin UI is single-user on the server in your setup.
    private static string? _globalCachedToken;

    public ProtectedLocalStorageTokenService(ProtectedLocalStorage protectedLocalStorage, IAppLogger logger, IJSRuntime js)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
        _js = js;
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            // Update in-memory cache first so server-side requests can use the token without JS interop
            _cachedToken = token;
            _globalCachedToken = token;

            await _protectedLocalStorage.SetAsync(TokenKey, token);
            _logger.LogInformation($"ProtectedLocalStorageTokenService: token stored (length={token?.Length ?? 0})");
        }
        catch (Exception ex)
        {
            _logger.LogError("ProtectedLocalStorageTokenService: failed to set token", ex);
            // fallback: attempt to write to plain localStorage for dev/debug
            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", DebugKey, token);
                _logger.LogWarning("ProtectedLocalStorageTokenService: fallback wrote token to localStorage debug key");
            }
            catch (Exception jsEx)
            {
                _logger.LogError("ProtectedLocalStorageTokenService: fallback write to localStorage failed", jsEx);
            }
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        // First check global cached token to support background handlers outside circuit scope
        if (!string.IsNullOrEmpty(_globalCachedToken))
        {
            _logger.LogDebug("ProtectedLocalStorageTokenService: returning global cached token");
            return _globalCachedToken;
        }

        // Return instance cached token immediately if available to avoid JS interop when it's not allowed
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            _logger.LogDebug("ProtectedLocalStorageTokenService: returning instance cached token");
            return _cachedToken;
        }

        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>(TokenKey);
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _logger.LogInformation("ProtectedLocalStorageTokenService: token retrieved (length={result.Value?.Length ?? 0})");
                _cachedToken = result.Value;
                _globalCachedToken = result.Value;
                return result.Value;
            }

            _logger.LogDebug("ProtectedLocalStorageTokenService: token not found in protected storage, attempting fallback to localStorage debug key");

            // Fallback: attempt to read from plain localStorage debug key (useful for debugging or when protected storage isn't available)
            try
            {
                var debugToken = await _js.InvokeAsync<string>("localStorage.getItem", DebugKey);
                if (!string.IsNullOrEmpty(debugToken))
                {
                    _logger.LogWarning("ProtectedLocalStorageTokenService: found debug token in localStorage (length={debugToken.Length}), migrating to protected storage");
                    // Try to migrate into protected storage
                    try
                    {
                        await _protectedLocalStorage.SetAsync(TokenKey, debugToken);
                        _logger.LogInformation("ProtectedLocalStorageTokenService: migrated debug token into protected storage");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("ProtectedLocalStorageTokenService: failed to migrate debug token into protected storage", ex);
                    }

                    _cachedToken = debugToken;
                    _globalCachedToken = debugToken;
                    return debugToken;
                }
            }
            catch (Exception jsEx)
            {
                _logger.LogError( "ProtectedLocalStorageTokenService: failed to read debug token from localStorage", jsEx);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("ProtectedLocalStorageTokenService: failed to get token from protected storage", ex);
            // As a last resort, try reading debug key from localStorage
            try
            {
                var debugToken = await _js.InvokeAsync<string>("localStorage.getItem", DebugKey);
                if (!string.IsNullOrEmpty(debugToken))
                {
                    _logger.LogError("ProtectedLocalStorageTokenService: recovered debug token from localStorage after exception");
                    _cachedToken = debugToken;
                    _globalCachedToken = debugToken;
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
            // Clear in-memory cache first
            _cachedToken = null;
            _globalCachedToken = null;

            await _protectedLocalStorage.DeleteAsync(TokenKey);
            _logger.LogInformation("ProtectedLocalStorageTokenService: token removed");
        }
        catch (Exception ex)
        {
            _logger.LogError("ProtectedLocalStorageTokenService: failed to remove token from protected storage", ex);
        }

        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", DebugKey);
            _logger.LogInformation("ProtectedLocalStorageTokenService: debug token removed from localStorage");
        }
        catch { }
    }
}
