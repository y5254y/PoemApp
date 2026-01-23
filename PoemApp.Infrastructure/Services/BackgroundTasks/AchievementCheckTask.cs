using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoemApp.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PoemApp.Infrastructure.Services.BackgroundTasks;

/// <summary>
/// 定时任务：成就检查
/// </summary>
public class AchievementCheckTask : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AchievementCheckTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // TODO: Implement achievement checking logic
                Console.WriteLine("Checking achievements...");

                // Example: Check for continuous check-in achievement
                // var users = dbContext.Users.ToList();
                // foreach (var user in users)
                // {
                //     // Check and update achievements
                // }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken); // 每6小时检查一次
        }
    }
}