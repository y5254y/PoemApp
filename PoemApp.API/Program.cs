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


            // ע����֤����
            // ��ӻ�����ʩ�����滻ԭ���ķ���ע�ᣩ
            builder.Services.AddInfrastructure(builder.Configuration);
           
            builder.Services.AddOpenApiDocument(config =>
            {
                config.Title = "PoemApp API";
                config.Version = "v1";
            });

            var app = builder.Build();

            // �� app ���ò����滻
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();    // ��Ҫ�������֤�м��
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
