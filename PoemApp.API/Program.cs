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


            // 注册认证服务
            // 添加基础设施服务（替换原来的服务注册）
            builder.Services.AddInfrastructure(builder.Configuration);
           
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
                app.UseSwaggerUi();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();    // 重要：添加认证中间件
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
