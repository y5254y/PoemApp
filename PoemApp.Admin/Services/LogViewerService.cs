using PoemApp.Core.Interfaces;

namespace PoemApp.Admin.Services;

public interface ILogViewerService
{
    Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string level = null);
    Task<string> GetLogFileContentAsync();
    Task ClearLogsAsync();
}

public class LogViewerService : ILogViewerService
{
    private readonly IApiService _apiService;
    private readonly IAppLogger _logger;

    public LogViewerService(IApiService apiService, IAppLogger logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string level = null)
    {
        try
        {
            // 这里可以从API获取日志，或者直接读取本地文件
            // 暂时返回模拟数据
            return await GetLogsFromFileAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"获取日志失败: {ex.Message}", ex);
            return new List<LogEntry>();
        }
    }

    public async Task<string> GetLogFileContentAsync()
    {
        try
        {
            // 读取日志文件内容
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");
            if (File.Exists(logPath))
            {
                return await File.ReadAllTextAsync(logPath);
            }
            return "日志文件不存在";
        }
        catch (Exception ex)
        {
            _logger.LogError($"读取日志文件失败: {ex.Message}", ex);
            return $"读取日志文件失败: {ex.Message}";
        }
    }

    public async Task ClearLogsAsync()
    {
        try
        {
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
                _logger.LogInformation("日志文件已清空");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"清空日志失败: {ex.Message}", ex);
            throw;
        }
    }

    private async Task<List<LogEntry>> GetLogsFromFileAsync()
    {
        var logs = new List<LogEntry>();
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "poemapp-admin.log");

        if (!File.Exists(logPath))
            return logs;

        var lines = await File.ReadAllLinesAsync(logPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var logEntry = ParseLogLine(line);
            if (logEntry != null)
            {
                logs.Add(logEntry);
            }
        }

        return logs.OrderByDescending(l => l.Timestamp).ToList();
    }


    private LogEntry ParseLogLine(string line)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            // 解析日志行格式: "2024-01-01 12:00:00.000 [Info] Category: Message"
            var timestampEnd = line.IndexOf(' ', 11);
            if (timestampEnd < 0)
                return new LogEntry { FullText = line, Timestamp = DateTime.Now, Level = "Unknown", Message = line };

            var timestampStr = line.Substring(0, timestampEnd);
            if (!DateTime.TryParse(timestampStr, out var timestamp))
                timestamp = DateTime.Now;

            var levelStart = line.IndexOf('[', timestampEnd);
            var levelEnd = line.IndexOf(']', levelStart + 1);

            string level = "Info";
            string message = line;

            if (levelStart >= 0 && levelEnd >= 0 && levelEnd > levelStart)
            {
                level = line.Substring(levelStart + 1, levelEnd - levelStart - 1);
                var messageStart = line.IndexOf(':', levelEnd);
                if (messageStart >= 0)
                {
                    message = line.Substring(messageStart + 1).Trim();
                }
            }

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Message = message,
                FullText = line
            };
        }
        catch (Exception)
        {
            return new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = "Error",
                Message = "Failed to parse log line",
                FullText = line
            };
        }
    }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string FullText { get; set; }

    //public string LevelColor => Level?.ToLower() switch
    //{
    //    "error" or "critical" => "error",
    //    "warning" => "warning",
    //    "information" or "info" => "info",
    //    "debug" => "secondary",
    //    _ => "default"
    //};
}