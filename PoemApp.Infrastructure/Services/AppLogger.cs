using Microsoft.Extensions.Logging;
using PoemApp.Core.Interfaces;

namespace PoemApp.Infrastructure.Services;

public class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;

    public AppLogger(ILogger<AppLogger> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    public void LogError(string message, Exception ex = null)
    {
        _logger.LogError(ex, message);
    }

    public void LogDebug(string message)
    {
        _logger.LogDebug(message);
    }

    public void LogCritical(string message, Exception ex = null)
    {
        _logger.LogCritical(ex, message);
    }
}