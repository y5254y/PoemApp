using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi;
using PoemApp.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using PoemApp.Infrastructure.Extensions;
using PoemApp.Infrastructure.Services;
using PoemApp.Core.Interfaces;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PoemApp.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 添加详细的日志记录
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        Console.WriteLine("=== API 项目启动 ===");
        Console.WriteLine($"环境: {builder.Environment.EnvironmentName}");
        Console.WriteLine($"根路径: {Directory.GetCurrentDirectory()}");

        // 注册基础设施服务
        try
        {
            builder.Services.AddInfrastructure(builder.Configuration);
            Console.WriteLine("基础设施服务注册成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"基础设施服务注册失败: {ex.Message}");
            Console.WriteLine($"异常详情: {ex}");
        }
        // 检查 IAuthService 是否已注册
        var serviceCollection = builder.Services;
        var authServiceDescriptor = serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(IAuthService));
        Console.WriteLine($"IAuthService 注册状态: {(authServiceDescriptor != null ? "已注册" : "未注册")}");

        // 如果是已注册，显示实现类型
        if (authServiceDescriptor != null)
        {
            Console.WriteLine($"IAuthService 实现类型: {authServiceDescriptor.ImplementationType?.Name}");
        }

        // 添加服务到容器
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();


        // 2. JWT认证配置（保留在API层）
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"]
                };
            });


        

        builder.Services.AddOpenApiDocument(config =>
        {
            config.Title = "PoemApp API";
            config.Version = "v1";
        });

        // 配置CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();


        // 种子数据（仅在开发环境）
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
            using var scope = app.Services.CreateScope();
            try
            {
                var seedService = scope.ServiceProvider.GetRequiredService<IDataSeedService>();
                await seedService.SeedTestDataAsync();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "种子数据执行失败");
            }
        }

        //app.UseHttpsRedirection();
        app.UseAuthentication();    // 重要：添加认证中间件
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

       

        app.Run();
    }
}
