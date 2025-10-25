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

        // �����ϸ����־��¼
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        Console.WriteLine("=== API ��Ŀ���� ===");
        Console.WriteLine($"����: {builder.Environment.EnvironmentName}");
        Console.WriteLine($"��·��: {Directory.GetCurrentDirectory()}");

        // ע�������ʩ����
        try
        {
            builder.Services.AddInfrastructure(builder.Configuration);
            Console.WriteLine("������ʩ����ע��ɹ�");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ʩ����ע��ʧ��: {ex.Message}");
            Console.WriteLine($"�쳣����: {ex}");
        }
        // ��� IAuthService �Ƿ���ע��
        var serviceCollection = builder.Services;
        var authServiceDescriptor = serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(IAuthService));
        Console.WriteLine($"IAuthService ע��״̬: {(authServiceDescriptor != null ? "��ע��" : "δע��")}");

        // �������ע�ᣬ��ʾʵ������
        if (authServiceDescriptor != null)
        {
            Console.WriteLine($"IAuthService ʵ������: {authServiceDescriptor.ImplementationType?.Name}");
        }

        // ��ӷ�������
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();


        // 2. JWT��֤���ã�������API�㣩
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

        // ����CORS
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


        // �������ݣ����ڿ���������
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
                logger.LogError(ex, "��������ִ��ʧ��");
            }
        }

        //app.UseHttpsRedirection();
        app.UseAuthentication();    // ��Ҫ�������֤�м��
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

       

        app.Run();
    }
}
