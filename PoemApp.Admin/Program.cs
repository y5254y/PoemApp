using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using PoemApp.Admin.Services;
using PoemApp.Core.Interfaces;
using PoemApp.Infrastructure.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using PoemApp.Client.ApiClients;

var builder = WebApplication.CreateBuilder(args);

// Create a LoggingLevelSwitch so we can change the level at runtime
var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

// Configure Serilog with async file sink and controlled level
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Async(a => a.File(logFilePath, rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Host.UseSerilog();

// expose Serilog.ILogger and LoggingLevelSwitch to DI so components can change level
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddSingleton<LoggingLevelSwitch>(levelSwitch);

// 注册自定义日志服务
builder.Services.AddSingleton<IAppLogger, AppLogger>();

builder.Services.AddRazorPages();
// Register Blazor Server once with circuit options
builder.Services.AddServerSideBlazor()
        .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddMudServices();

// Register ProtectedLocalStorage service manually
builder.Services.AddScoped<ProtectedLocalStorage>();

// Authentication/Authorization for Blazor
// Use AddAuthorizationCore without setting a global FallbackPolicy to avoid server-side Challenge errors
builder.Services.AddAuthorizationCore();
// Register only the abstraction to avoid multiple instances; concrete type will be created by DI
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// Use shared AddPoemAppApiClients - configure base URL from configuration
var apiBase = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5001/";
builder.Services.AddPoemAppApiClients(apiBase);

// Register API clients and auth service (if additional admin-specific clients exist)
builder.Services.AddScoped<PoemApiClient>();
// AuthorsApiClient, CategoriesApiClient, UsersApiClient now provided by shared library registration
builder.Services.AddScoped<AdminAuthService>();

// Token service (use ProtectedLocalStorage in Blazor Server)
builder.Services.AddScoped<ITokenService, ProtectedLocalStorageTokenService>();
    
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

// Ensure text/html responses explicitly include utf-8 charset to avoid mojibake when served through Docker
app.Use(async (context, next) =>
{
    context.Response.OnStarting(state =>
    {
        var httpContext = (HttpContext)state!;
        var ct = httpContext.Response.ContentType;
        if (!string.IsNullOrEmpty(ct) && ct.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) && !ct.Contains("charset", StringComparison.OrdinalIgnoreCase))
        {
            httpContext.Response.ContentType = ct + "; charset=utf-8";
        }
        return Task.CompletedTask;
    }, context);

    await next();
});

app.UseRouting();

// Admin minimal endpoints for logging control and reading logs
app.MapGet("/admin/logging", (LoggingLevelSwitch ls) => Results.Ok(new { Level = ls.MinimumLevel.ToString() }));

app.MapPost("/admin/logging/level", (LoggingLevelSwitch ls, HttpRequest req) =>
{
    var level = req.Query["level"].ToString();
    if (!Enum.TryParse<LogEventLevel>(level, true, out var parsed))
        return Results.BadRequest("Invalid level");

    ls.MinimumLevel = parsed;
    return Results.Ok(new { Level = ls.MinimumLevel.ToString() });
});

app.MapGet("/admin/logs", (int lines) =>
{
    try
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logDir))
            return Results.NotFound("Log directory not found");

        var files = Directory.GetFiles(logDir, "poemapp-admin*.log");
        if (files.Length == 0)
            return Results.NotFound("No admin log files found");

        var latest = files.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).First();
        var allLines = System.IO.File.ReadAllLines(latest);
        if (lines <= 0 || lines >= allLines.Length)
        {
            return Results.Text(string.Join("\n", allLines));
        }
        else
        {
            var tail = allLines.Skip(Math.Max(0, allLines.Length - lines));
            return Results.Text(string.Join("\n", tail));
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
