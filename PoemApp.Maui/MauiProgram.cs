using Microsoft.Extensions.Logging;
using PoemApp.Client.ApiClients;

namespace PoemApp.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Register HttpClient for calling the API. Change BaseAddress to your API URL if needed.
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("http://172.16.1.227:7000/");
            });

            // Register API client and pages for DI
            builder.Services.AddTransient<PoemApiClient>();
            builder.Services.AddTransient<Views.HomePage>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
