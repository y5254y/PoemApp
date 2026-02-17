using Microsoft.Extensions.DependencyInjection;
using System;
namespace PoemApp.Client.ApiClients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPoemAppApiClients(this IServiceCollection services, string apiBase)
    {
        services.AddScoped<AuthHttpMessageHandler>();
        services.AddHttpClient("Api", client =>
        {
            client.BaseAddress = new Uri(apiBase);
        }).AddHttpMessageHandler<AuthHttpMessageHandler>();

        services.AddScoped<AuthorsApiClient>();
        services.AddScoped<CategoriesApiClient>();
        services.AddScoped<UsersApiClient>();
        services.AddScoped<PoemApiClient>();
        services.AddScoped<QuotesApiClient>();
        services.AddScoped<PoemFeedbacksApiClient>();
        services.AddScoped<QuoteFeedbacksApiClient>();

        return services;
    }
}