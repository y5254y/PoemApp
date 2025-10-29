using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Admin.Services;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace PoemApp.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAdminAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            _logger.LogInformation("GET Login 方法被调用，ReturnUrl: {ReturnUrl}", returnUrl);

            // 如果用户已认证，直接重定向
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("用户已认证，重定向到首页");
                return Redirect("/"); // 修改这里：改为重定向到根路径
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Login")] // 添加这个特性
        [Route("Account/Login")] // 显式指定路由
        public async Task<IActionResult> LoginPost() // 修改方法名以避免冲突
        {
            _logger.LogInformation("POST Login 方法被调用");

            // 手动绑定模型
            var username = Request.Form["Username"].ToString();
            var password = Request.Form["Password"].ToString();
            var returnUrl = Request.Form["returnUrl"].ToString();

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";

            ViewData["ReturnUrl"] = returnUrl;

            _logger.LogInformation("手动绑定 - Username: {Username}, Password: {Password}, ReturnUrl: {ReturnUrl}",
                username, password, returnUrl);

            // 创建模型对象
            var model = new LoginDto
            {
                Username = username,
                Password = password
            };

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                _logger.LogWarning("用户名或密码为空");
                ModelState.AddModelError(string.Empty, "用户名和密码不能为空");
                return View("Login", model);
            }

            try
            {
                _logger.LogInformation("开始用户认证: {Username}", model.Username);

                var result = await _authService.LoginAsync(model.Username, model.Password);

                if (result?.Success == true)
                {
                    _logger.LogInformation("用户认证成功: {Username}", model.Username);

                    // 设置认证 Cookie
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.User?.Username ?? model.Username),
                        new Claim(ClaimTypes.NameIdentifier, result.User?.Id.ToString() ?? ""),
                        new Claim(ClaimTypes.Role, result.User?.Role.ToString() ?? "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // 修改重定向逻辑 - 重定向到根路径而不是 Home/Index
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        _logger.LogInformation("重定向到 ReturnUrl: {ReturnUrl}", returnUrl);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogInformation("重定向到首页");
                        return Redirect("/"); // 修改这里：改为重定向到根路径
                    }
                }
                else
                {
                    _logger.LogWarning("用户认证失败: {Username}, 原因: {Message}", model.Username, result?.Message);
                    ModelState.AddModelError(string.Empty, result?.Message ?? "登录失败，请检查用户名和密码");
                    return View("Login", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录过程中发生异常: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, $"登录失败: {ex.Message}");
                return View("Login", model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("用户注销");

            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}