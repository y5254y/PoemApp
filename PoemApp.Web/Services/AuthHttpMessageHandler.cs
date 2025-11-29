using System.Net.Http.Headers;
using System.Threading;
using PoemApp.Core.Interfaces;

namespace PoemApp.Web.Services;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthHttpMessageHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
