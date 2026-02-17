using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using PoemApp.Core.Enums;

namespace PoemApp.Client.ApiClients;

public class QuoteFeedbacksApiClient
{
    private readonly HttpClient _http;

    public QuoteFeedbacksApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<(List<QuoteFeedbackDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, FeedbackStatus? status = null)
    {
        var url = $"api/quotefeedbacks?pageNumber={pageNumber}&pageSize={pageSize}";
        if (status.HasValue)
        {
            url += $"&status={(int)status.Value}";
        }

        var response = await _http.GetFromJsonAsync<FeedbackListResponse<QuoteFeedbackDto>>(url);
        return response == null
            ? (new List<QuoteFeedbackDto>(), 0)
            : (response.Data ?? new List<QuoteFeedbackDto>(), response.Total);
    }

    private sealed class FeedbackListResponse<T>
    {
        public int Total { get; set; }
        public List<T>? Data { get; set; }
    }
}
