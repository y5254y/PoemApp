using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using PoemApp.Admin.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Register ProtectedLocalStorage service manually
builder.Services.AddScoped<ProtectedLocalStorage>();

// Authentication/Authorization for Blazor
builder.Services.AddAuthorizationCore();
// Register only the abstraction to avoid multiple instances; concrete type will be created by DI
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// HttpClient for API calls
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5001/");
});
builder.Services.AddServerSideBlazor()
        .AddCircuitOptions(options => { options.DetailedErrors = true; });


// Register API clients and auth service
builder.Services.AddScoped<PoemApiClient>();
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
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
