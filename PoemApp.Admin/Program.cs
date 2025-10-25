using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using PoemApp.Admin.Services;
using PoemApp.Core.Interfaces;
using PoemApp.Admin.Components;

var builder = WebApplication.CreateBuilder(args);

// 配置Kestrel使用7002端口
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7002, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
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

builder.Services.AddScoped<ApiServiceConfiguration>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LoginDtoValidation>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// 添加HTTP上下文访问
builder.Services.AddHttpContextAccessor();

// 添加认证
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "poemapp.admin.auth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
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

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
