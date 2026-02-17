using System.Net.Http.Json;
using PoemApp.Core.DTOs;
using PoemApp.Core.Enums;

namespace PoemApp.Client.ApiClients;

public class PoemFeedbacksApiClient
{
    private readonly HttpClient _http;

    public PoemFeedbacksApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<(List<PoemFeedbackDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, FeedbackStatus? status = null)
    {
        var url = $"api/poemfeedbacks?pageNumber={pageNumber}&pageSize={pageSize}";
        if (status.HasValue)
        {
            url += $"&status={(int)status.Value}";
        }

        var response = await _http.GetFromJsonAsync<FeedbackListResponse<PoemFeedbackDto>>(url);
        return response == null
            ? (new List<PoemFeedbackDto>(), 0)
            : (response.Data ?? new List<PoemFeedbackDto>(), response.Total);
    }

    private sealed class FeedbackListResponse<T>
    {
        public int Total { get; set; }
        public List<T>? Data { get; set; }
    }
}
