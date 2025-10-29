using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using PoemApp.Admin.Components;
using PoemApp.Admin.Services;
using PoemApp.Core.Interfaces;
using PoemApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 配置Kestrel使用7002端口
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(7002, listenOptions =>
//    {
//        listenOptions.UseHttps();
//    });
//});
// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 添加文件日志提供程序
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");
builder.Logging.AddProvider(new FileLoggerProvider(logFilePath));

// 注册自定义日志服务
builder.Services.AddScoped<IAppLogger, AppLogger>();
builder.Services.AddScoped<ILogViewerService, LogViewerService>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
//builder.Services.AddSingleton<WeatherForecastService>();

// 添加 HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>(sp =>
{
    //var handler = new HttpClientHandler();
    //// 忽略证书验证（开发环境使用）
    //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    var config = sp.GetRequiredService<ApiServiceConfiguration>();
    var client = new HttpClient { BaseAddress = new Uri(config.BaseUrl) };
    return client;
});



// 配置API服务
builder.Services.Configure<ApiServiceConfiguration>(options =>
{
    options.BaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001/api/";
});
// 添加 MVC 服务以支持控制器
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogViewerService, LogViewerService>();
builder.Services.AddScoped<ApiServiceConfiguration>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAdminAuthService ,AdminAuthService>();
//builder.Services.AddScoped<LoginDtoValidation>(); 不再使用blazor表单验证,改为控制器验证
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
// 注册仪表板服务
//builder.Services.AddScoped<IDashboardService, DashboardService>();
// 添加HTTP上下文访问
builder.Services.AddHttpContextAccessor();

// 添加模型绑定诊断
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

        foreach (var key in context.ModelState.Keys)
        {
            var state = context.ModelState[key];
            foreach (var error in state.Errors)
            {
                logger.LogWarning("模型绑定错误 - {Key}: {ErrorMessage}", key, error.ErrorMessage);
            }
        }

        return new BadRequestObjectResult(context.ModelState);
    };
});

// 添加认证
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "poemapp.admin.auth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 添加自定义错误处理中间件
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // 记录错误但不修改已开始的响应
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "全局异常处理");

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("内部服务器错误");
        }
    }
});

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// 添加控制器路由
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action}",
    defaults: new { controller = "Account" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
