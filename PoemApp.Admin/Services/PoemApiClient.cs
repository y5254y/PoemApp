using System.Net.Http.Json;
using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class PoemApiClient
{
    private readonly HttpClient _http;

    public PoemApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<List<PoemDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<PoemDto>>("api/poems") ?? new List<PoemDto>();
    }
}