using Microsoft.AspNetCore.Components.Authorization;
using PoemApp.Admin.Services;
using System.Security.Claims;

namespace PoemApp.Admin.Components
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAdminAuthService _authService;

        public CustomAuthStateProvider(IAdminAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            if (_authService.IsAuthenticated())
            {
                // 这里可以从AuthService获取用户信息创建Claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, _authService.GetUserName() ?? "用户"),
                    new Claim(ClaimTypes.Role, _authService.GetUserRole() ?? "User")
                };
                identity = new ClaimsIdentity(claims, "CustomAuth");
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}