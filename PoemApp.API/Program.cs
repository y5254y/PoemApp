using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PoemApp.API.Data;
using PoemApp.API.Services;
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

            // ��� DbContext ����
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                // ʹ�� SQLite
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // ���ʹ�� MySQL��ʹ�����´��룺
                // options.UseMySql(
                //     builder.Configuration.GetConnectionString("DefaultConnection"),
                //     new MySqlServerVersion(new Version(8, 0, 27))
                // );
            });
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
            builder.Services.AddScoped<IAuthService, AuthService>();
          
            builder.Services.AddScoped<IPoemService, PoemService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAnnotationService, AnnotationService>();
            builder.Services.AddScoped<IAudioService, AudioService>();
            builder.Services.AddScoped<IUserService, UserService>();
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
                //app.UseSwaggerUi3();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
