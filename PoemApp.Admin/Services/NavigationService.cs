using Microsoft.AspNetCore.Components;

namespace PoemApp.Admin.Services;

public interface INavigationService
{
    Task NavigateToAsync(string uri, bool forceLoad = false);
    Task NavigateToAsync(string uri, int delayMs, bool forceLoad = false);
}

public class NavigationService : INavigationService
{
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(NavigationManager navigationManager, ILogger<NavigationService> logger)
    {
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task NavigateToAsync(string uri, bool forceLoad = false)
    {
        await NavigateToAsync(uri, 0, forceLoad);
    }

    public async Task NavigateToAsync(string uri, int delayMs, bool forceLoad = false)
    {
        try
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            _navigationManager.NavigateTo(uri, forceLoad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导航到 {Uri} 时发生错误", uri);

            // 备用方案：使用 JavaScript 重定向
            await TryJavaScriptRedirect(uri);
        }
    }

    private async Task TryJavaScriptRedirect(string uri)
    {
        try
        {
            // 这里可以添加 JavaScript 重定向逻辑
            // 需要注入 IJSRuntime
            _logger.LogWarning("使用备用重定向方案到: {Uri}", uri);
            await Task.CompletedTask;
        }
        catch (Exception jsEx)
        {
            _logger.LogError(jsEx, "备用重定向方案也失败了");
        }
    }
}