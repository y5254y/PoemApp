using System.Net.Http.Headers;
using PoemApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace PoemApp.Client.ApiClients;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthHttpMessageHandler> _logger;

    public AuthHttpMessageHandler(ITokenService tokenService, ILogger<AuthHttpMessageHandler> logger)
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
                var display = token.Length > 10 ? token.Substring(0, 6) + "..." + token.Substring(token.Length - 4) : token;
                _logger.LogInformation("AuthHttpMessageHandler: attaching bearer token (preview={TokenPreview}) to request {Method} {Url}", display, request.Method, request.RequestUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogDebug("AuthHttpMessageHandler: no token available for request {Method} {Url}", request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthHttpMessageHandler: failed to get token, proceeding without Authorization header");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
