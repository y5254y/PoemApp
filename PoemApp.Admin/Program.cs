using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using PoemApp.Admin.Components;
using PoemApp.Admin.Services;
using PoemApp.Core.Interfaces;
using PoemApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ����Kestrelʹ��7002�˿�
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(7002, listenOptions =>
//    {
//        listenOptions.UseHttps();
//    });
//});
// ������־
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ����ļ���־�ṩ����
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");
builder.Logging.AddProvider(new FileLoggerProvider(logFilePath));

// ע���Զ�����־����
builder.Services.AddScoped<IAppLogger, AppLogger>();
builder.Services.AddScoped<ILogViewerService, LogViewerService>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
//builder.Services.AddSingleton<WeatherForecastService>();

// ��� HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>(sp =>
{
    //var handler = new HttpClientHandler();
    //// ����֤����֤����������ʹ�ã�
    //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    var config = sp.GetRequiredService<ApiServiceConfiguration>();
    var client = new HttpClient { BaseAddress = new Uri(config.BaseUrl) };
    return client;
});



// ����API����
builder.Services.Configure<ApiServiceConfiguration>(options =>
{
    options.BaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001/api/";
});
// ��� MVC ������֧�ֿ�����
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogViewerService, LogViewerService>();
builder.Services.AddScoped<ApiServiceConfiguration>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAdminAuthService ,AdminAuthService>();
//builder.Services.AddScoped<LoginDtoValidation>(); ����ʹ��blazor����֤,��Ϊ��������֤
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
// ע���Ǳ�����
//builder.Services.AddScoped<IDashboardService, DashboardService>();
// ���HTTP�����ķ���
builder.Services.AddHttpContextAccessor();

// ���ģ�Ͱ����
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
                logger.LogWarning("ģ�Ͱ󶨴��� - {Key}: {ErrorMessage}", key, error.ErrorMessage);
            }
        }

        return new BadRequestObjectResult(context.ModelState);
    };
});

// �����֤
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

// ����Զ���������м��
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // ��¼���󵫲��޸��ѿ�ʼ����Ӧ
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "ȫ���쳣����");

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("�ڲ�����������");
        }
    }
});

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// ��ӿ�����·��
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
