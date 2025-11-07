using System.Net.Http.Headers;
using PoemApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace PoemApp.Admin.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthMessageHandler> _logger;

    public AuthMessageHandler(ITokenService tokenService, ILogger<AuthMessageHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                // Log token presence but avoid printing full token
                var display = token.Length > 10 ? token.Substring(0, 6) + "..." + token.Substring(token.Length - 4) : token;
                // Use Information so it appears with default log level
                _logger.LogInformation("AuthMessageHandler: attaching bearer token (preview={TokenPreview}) to request {Method} {Url}", display, request.Method, request.RequestUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("AuthMessageHandler: no token available for request {Method} {Url}", request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthMessageHandler: failed to get token, proceeding without Authorization header");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
