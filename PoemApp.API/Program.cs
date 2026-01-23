using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PoemApp.Core.Interfaces;
//using Microsoft.OpenApi;
using PoemApp.Infrastructure.Data;
using PoemApp.Infrastructure.Extensions;
using PoemApp.Infrastructure.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text;


namespace PoemApp.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Create a LoggingLevelSwitch so we can change the level at runtime
        var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

        // Configure Serilog; file sink only for dev/local
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .Enrich.FromLogContext()
            .WriteTo.Console();

        var isContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);

        if (builder.Environment.IsDevelopment() || !isContainer)
        {
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-api.log");
            loggerConfig = loggerConfig.WriteTo.Async(a => a.File(logFilePath, rollingInterval: RollingInterval.Day));
        }

        Log.Logger = loggerConfig.CreateLogger();

        builder.Host.UseSerilog();

        // expose Serilog.ILogger and LoggingLevelSwitch to DI so controllers can change level
        builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
        builder.Services.AddSingleton<LoggingLevelSwitch>(levelSwitch);

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



        builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
        .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()

        //.AddConsoleExporter()
        //.AddOtlpExporter()

        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://tracing-analysis-dc-hz.aliyuncs.com/adapt_f1pfgeadvy@a75a87db5bc9eb3_f1pfgeadvy@53df7ad2afe8301/api/otlp/traces");
            otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;

        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        //.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
        //{
        //    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
        //})
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://tracing-analysis-dc-hz.aliyuncs.com/adapt_f1pfgeadvy@a75a87db5bc9eb3_f1pfgeadvy@53df7ad2afe8301/api/otlp/traces");
            otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;

        }));


        // 添加服务到容器
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // 添加 JWT 认证
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 64)
        {
            jwtKey = "ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatIsAtLeast64BytesLongToMeetHMACSHA512Requirements";
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    // Ensure role/name claim mapping matches tokens created in AuthService
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    // 放宽时钟偏差，避免时间同步问题
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"认证失败: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token 验证成功");
                        return Task.CompletedTask;
                    }
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
            var allowedOrigins = builder.Configuration.GetValue<string>("Cors:AllowedOrigins")?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new[] { "http://localhost:6000" };
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // 注册后台定时任务
        builder.Services.AddHostedService<PoemApp.Infrastructure.Services.BackgroundTasks.ReviewReminderTask>();
        builder.Services.AddHostedService<PoemApp.Infrastructure.Services.BackgroundTasks.ExpiredReviewTask>();
        builder.Services.AddHostedService<PoemApp.Infrastructure.Services.BackgroundTasks.AchievementCheckTask>();

        var app = builder.Build();

        // 自动应用 EF Core 迁移以确保数据库模式包含新添加的字段（Category.SortOrder/IsEnabled/IsLeaf 等）
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<PoemApp.Infrastructure.Data.AppDbContext>();
                if (db != null)
                {
                    Console.WriteLine("Applying database migrations...");
                    db.Database.Migrate();
                    Console.WriteLine("Database migrations applied.");
                    try
                    {
                        // Ensure new category columns exist (safe for MySQL 8+ with IF NOT EXISTS)
                        db.Database.ExecuteSqlRaw(@"ALTER TABLE `categories` ADD COLUMN IF NOT EXISTS `sortorder` int NOT NULL DEFAULT 0;");
                        db.Database.ExecuteSqlRaw(@"ALTER TABLE `categories` ADD COLUMN IF NOT EXISTS `isenabled` tinyint(1) NOT NULL DEFAULT 1;");
                        db.Database.ExecuteSqlRaw(@"ALTER TABLE `categories` ADD COLUMN IF NOT EXISTS `isleaf` tinyint(1) NOT NULL DEFAULT 1;");
                        Console.WriteLine("Ensured category extra columns exist.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ensure category columns failed: " + ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to apply migrations: " + ex.Message);
        }

        // 在应用启动时自动执行 EF Core 迁移，确保数据库结构与迁移文件同步。
        try
        {
            using var migrateScope = app.Services.CreateScope();
            var migrateDb = migrateScope.ServiceProvider.GetRequiredService<AppDbContext>();
            migrateDb.Database.Migrate();
            var migrateLogger = migrateScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            migrateLogger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            // 记录迁移失败但不阻止应用启动（根据需要可改为抛出以阻止启动）
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while applying database migrations.");
        }


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

        //app.MapHealthChecks("/health"); // Map health check endpoint

        app.Run();
    }
}
