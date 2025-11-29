using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PoemApp.Web;
using PoemApp.Web.Services;
using PoemApp.Client.ApiClients;
using PoemApp.Core.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Determine API base: prefer explicit configuration
var configuredApiBase = builder.Configuration["ApiBaseUrl"];
var apiBase = !string.IsNullOrWhiteSpace(configuredApiBase) ? configuredApiBase : builder.HostEnvironment.BaseAddress;

// Register shared API clients
builder.Services.AddPoemAppApiClients(apiBase);

// Add MudBlazor
builder.Services.AddMudServices();

// Add auth/local token service
builder.Services.AddScoped<ITokenService, LocalStorageTokenService>();

// Register token-dependent Auth handler is already added by AddPoemAppApiClients

await builder.Build().RunAsync();
