using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MySql.EntityFrameworkCore.Extensions;

using System.IO;

namespace PoemApp.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 获取当前目录
        var basePath = Directory.GetCurrentDirectory();

        // Try to locate appsettings.json by searching up the directory tree and also check common project locations
        string? configPath = null;
        var searchPaths = new[] {
            basePath,
            Path.Combine(basePath, "PoemApp.API"),
            Path.Combine(basePath, "..", "PoemApp.API"),
            Path.Combine(basePath, "..", "..", "PoemApp.API")
        };

        foreach (var p in searchPaths)
        {
            try
            {
                var full = Path.GetFullPath(p);
                var candidate = Path.Combine(full, "appsettings.json");
                if (File.Exists(candidate))
                {
                    configPath = candidate;
                    basePath = full;
                    break;
                }
            }
            catch { }
        }

        // 构建配置
        var builderConfig = new ConfigurationBuilder()
            .SetBasePath(basePath);

        if (configPath != null)
        {
            // load the specific file from found location
            builderConfig.AddJsonFile(Path.GetFileName(configPath));
        }
        else
        {
            // fall back to default (may throw later if no connection string)
            builderConfig.AddJsonFile("appsettings.json", optional: true);
        }

        var configuration = builderConfig.Build();

        // 构建 DbContextOptions
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 使用 SQLite
        //builder.UseSqlite(connectionString);

        // 如果使用官方 MySQL provider，调用 UseMySQL（某些 provider 使用不同方法名；请根据安装的包调整）
        builder.UseMySQL(connectionString);

        return new AppDbContext(builder.Options);
    }
}