using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace PoemApp.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 获取当前目录
        var basePath = Directory.GetCurrentDirectory();

        // 构建配置
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        // 构建 DbContextOptions
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 使用 SQLite
        builder.UseSqlite(connectionString);

        // 如果使用 MySQL，使用以下代码：
        // builder.UseMySql(
        //     connectionString,
        //     new MySqlServerVersion(new Version(8, 0, 27))
        // );

        return new AppDbContext(builder.Options);
    }
}