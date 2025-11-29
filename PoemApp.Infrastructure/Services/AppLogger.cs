using Serilog;
using PoemApp.Core.Interfaces;

namespace PoemApp.Infrastructure.Services;

public class AppLogger : IAppLogger
{
    private readonly ILogger _logger;

    public AppLogger(ILogger logger)
    {
        // Ensure we have a context-specific logger
        _logger = logger.ForContext<AppLogger>();
    }

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogError(string message, Exception? ex = null)
    {
        if (ex == null)
            _logger.Error(message);
        else
            _logger.Error(ex, message);
    }

    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    public void LogCritical(string message, Exception? ex = null)
    {
        if (ex == null)
            _logger.Fatal(message);
        else
            _logger.Fatal(ex, message);
    }
}