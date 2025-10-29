using PoemApp.Core.DTOs;
using System.Net.Http.Json;

namespace PoemApp.Admin.Services;

public class ApiServiceConfiguration
{
    public string BaseUrl { get; set; } = "http://localhost:7001/api/";
    public string? Token { get; set; }
}

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    void SetToken(string token);
    string? GetToken();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiServiceConfiguration _config;
    private readonly ILogger<ApiService> _logger;
    public ApiService(HttpClient httpClient, ApiServiceConfiguration config, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;

        // 开发环境：忽略 SSL 证书验证
#if DEBUG
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _httpClient = new HttpClient(handler);
#endif

        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PoemApp.Admin");
    }

    public void SetToken(string token)
    {
        _config.Token = token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public string? GetToken()
    {
        return _config.Token;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            Console.WriteLine($"API GET Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API GET Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            Console.WriteLine($"API POST Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API POST Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            Console.WriteLine($"API PUT Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API PUT Error: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API DELETE Error: {ex.Message}");
            return false;
        }
    }
}