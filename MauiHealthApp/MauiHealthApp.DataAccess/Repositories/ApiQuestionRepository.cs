using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiHealthApp.DataAccess.Repositories;

public class ApiQuestionRepository : IApiQuestionRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiQuestionRepository> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiQuestionRepository(IHttpClientFactory httpClientFactory, ILogger<ApiQuestionRepository> logger)
    {
        _httpClient = httpClientFactory.CreateClient("HealthApi");
        _logger = logger;
    }

    public async Task<(List<QuestionDto> Items, int Total)> GetPagedAsync(Guid userId, int page, int pageSize, string? search)
    {
        var url = $"api/questions?userId={userId}&page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiPagedResponse<QuestionDto>>(_jsonOptions);
        return (result?.Data?.Items ?? new(), result?.Data?.TotalCount ?? 0);
    }

    public async Task<QuestionDto?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/questions/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<QuestionDto>>(_jsonOptions);
        return result?.Data;
    }

    public async Task<Guid> CreateAsync(CreateQuestionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/questions", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<QuestionDto>>(_jsonOptions);
        return result?.Data?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateQuestionRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/questions/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/questions/{id}");
        return response.IsSuccessStatusCode;
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    private class ApiPagedResponse<T>
    {
        public bool Success { get; set; }
        public PagedData<T>? Data { get; set; }
    }

    private class PagedData<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
