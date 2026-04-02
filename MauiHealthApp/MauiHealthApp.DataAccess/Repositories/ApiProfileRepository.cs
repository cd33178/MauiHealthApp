using MauiHealthApp.DataAccess.Repositories;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiHealthApp.DataAccess.Repositories;

public class ApiProfileRepository : IApiProfileRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiProfileRepository> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiProfileRepository(IHttpClientFactory httpClientFactory, ILogger<ApiProfileRepository> logger)
    {
        _httpClient = httpClientFactory.CreateClient("HealthApi");
        _logger = logger;
    }

    public async Task<ProfileDto?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/profiles/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProfileDto>>(_jsonOptions);
        return result?.Data;
    }

    public async Task<ProfileDto?> GetByUserIdAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"api/profiles/user/{userId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProfileDto>>(_jsonOptions);
        return result?.Data;
    }

    public async Task<Guid> CreateAsync(CreateProfileRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/profiles", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProfileDto>>(_jsonOptions);
        return result?.Data?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProfileRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/profiles/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/profiles/{id}");
        return response.IsSuccessStatusCode;
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}
