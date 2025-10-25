using PoemApp.Admin.Models;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IApiService _apiService;

    public DashboardService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            // 这里应该调用API的统计端点
            // 暂时返回模拟数据
            return new DashboardStats
            {
                PoemCount = await GetPoemCountAsync(),
                AuthorCount = await GetAuthorCountAsync(),
                UserCount = await GetUserCountAsync(),
                AudioCount = await GetAudioCountAsync(),
                AnnotationCount = await GetAnnotationCountAsync(),
                CategoryCount = await GetCategoryCountAsync()
            };
        }
        catch (Exception)
        {
            // 如果API调用失败，返回默认数据
            return new DashboardStats
            {
                PoemCount = 0,
                AuthorCount = 0,
                UserCount = 0,
                AudioCount = 0,
                AnnotationCount = 0,
                CategoryCount = 0
            };
        }
    }

    private async Task<int> GetPoemCountAsync()
    {
        var poems = await _apiService.GetAsync<List<PoemDto>>("poems");
        return poems?.Count ?? 0;
    }

    private async Task<int> GetAuthorCountAsync()
    {
        var authors = await _apiService.GetAsync<List<AuthorDto>>("authors");
        return authors?.Count ?? 0;
    }

    private async Task<int> GetUserCountAsync()
    {
        var users = await _apiService.GetAsync<List<UserDto>>("users");
        return users?.Count ?? 0;
    }

    private async Task<int> GetAudioCountAsync()
    {
        var audios = await _apiService.GetAsync<List<AudioDto>>("audios");
        return audios?.Count ?? 0;
    }

    private async Task<int> GetAnnotationCountAsync()
    {
        var annotations = await _apiService.GetAsync<List<AnnotationDto>>("annotations");
        return annotations?.Count ?? 0;
    }

    private async Task<int> GetCategoryCountAsync()
    {
        var categories = await _apiService.GetAsync<List<CategoryDto>>("categories");
        return categories?.Count ?? 0;
    }
}