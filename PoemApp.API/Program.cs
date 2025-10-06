using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PoemApp.API.Data;
using PoemApp.API.Services;
using PoemApp.Core.Interfaces;
using System.Reflection;
namespace PoemApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加服务到容器
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // 添加 DbContext 配置
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                // 使用 SQLite
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // 如果使用 MySQL，使用以下代码：
                // options.UseMySql(
                //     builder.Configuration.GetConnectionString("DefaultConnection"),
                //     new MySqlServerVersion(new Version(8, 0, 27))
                // );
            });

            // 注册自定义服务
            builder.Services.AddScoped<IPoemService, PoemService>();
            builder.Services.AddOpenApiDocument(config =>
            {
                config.Title = "PoemApp API";
                config.Version = "v1";
            });

            var app = builder.Build();

            // 在 app 配置部分替换
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                //app.UseSwaggerUi3();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
