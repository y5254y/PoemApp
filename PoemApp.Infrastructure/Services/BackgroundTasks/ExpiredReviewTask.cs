using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoemApp.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PoemApp.Infrastructure.Services.BackgroundTasks;

/// <summary>
/// 定时任务：过期复习处理
/// </summary>
public class ExpiredReviewTask : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ExpiredReviewTask(IServiceProvider serviceProvider)
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
                var now = DateTime.UtcNow;

                var expiredReviews = dbContext.RecitationReviews
                    .Where(rr => rr.ScheduledTime.AddHours(48) <= now && rr.Status == Core.Enums.ReviewStatus.Pending)
                    .ToList();

                foreach (var review in expiredReviews)
                {
                    review.Status = Core.Enums.ReviewStatus.Expired;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // 每天检查一次
        }
    }
}