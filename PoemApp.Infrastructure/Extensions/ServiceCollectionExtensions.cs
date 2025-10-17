// Infrastructure/Extensions/ServiceCollectionExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoemApp.Infrastructure.Data;
using PoemApp.Infrastructure.Services;
using PoemApp.Core.Interfaces;


namespace PoemApp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册 DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // 注册服务实现
            services.AddScoped<IPoemService, PoemService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<IAnnotationService, AnnotationService>();

            return services;
        }
    }
}