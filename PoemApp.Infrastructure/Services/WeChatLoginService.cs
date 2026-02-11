using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;
using PoemApp.Infrastructure.Settings;

namespace PoemApp.Infrastructure.Services
{
    public interface IWeChatLoginService
    {
        Task<WeChatCode2SessionResponseDto?> Code2SessionAsync(string code);
    }

    public class WeChatLoginService : IWeChatLoginService
    {
        private readonly HttpClient _httpClient;
        private readonly WeChatSettings _settings;
        private readonly IAppLogger _logger;

        public WeChatLoginService(HttpClient httpClient, IOptions<WeChatSettings> settings, IAppLogger logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<WeChatCode2SessionResponseDto?> Code2SessionAsync(string code)
        {
            var url = $"{_settings.MiniApp.Code2SessionUrl}?appid={_settings.MiniApp.AppId}&secret={_settings.MiniApp.AppSecret}&js_code={code}&grant_type=authorization_code";
            try
            {
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"WeChat code2session 返回非成功状态: {resp.StatusCode}");
                    return null;
                }

                var text = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<WeChatCode2SessionResponseDto>(text, options);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"WeChat Code2Session 调用失败: {ex.Message}", ex);
                return null;
            }
        }
    }
}
