using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoemApp.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PoemApp.Infrastructure.Services.BackgroundTasks;

/// <summary>
/// 定时任务：复习提醒
/// </summary>
public class ReviewReminderTask : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ReviewReminderTask(IServiceProvider serviceProvider)
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

                var dueReviews = dbContext.RecitationReviews
                    .Where(rr => rr.ScheduledTime <= now && rr.Status == Core.Enums.ReviewStatus.Pending && !rr.ReminderSent)
                    .ToList();

                foreach (var review in dueReviews)
                {
                    // TODO: Implement notification logic (e.g., email, push notification)
                    Console.WriteLine($"Reminder: Review ID {review.Id} is due.");

                    review.ReminderSent = true;
                    review.ReminderSentTime = now;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // 每小时检查一次
        }
    }
}