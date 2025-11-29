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
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Console.WriteLine("开始注册基础设施服务...");

            // 1. 首先注册数据库上下文
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"数据库连接字符串: {connectionString}");
                options.UseSqlite(connectionString);
            });

            // 2. 注册所有服务（按照依赖顺序）
            // 注册自定义日志服务
            services.AddSingleton<IAppLogger, AppLogger>();
            
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPoemService, PoemService>();
            services.AddTransient<IAuthorService, AuthorService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IAnnotationService, AnnotationService>();
            services.AddTransient<IAudioService, AudioService>();
            services.AddTransient<IDataSeedService, DataSeedService>();

            Console.WriteLine("基础设施服务注册完成");

            return services;
        }
    }
}