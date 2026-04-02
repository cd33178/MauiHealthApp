namespace MauiHealthApp.Services;

public interface IAuthService
{
    Task<bool> LoginAsync();
    Task LogoutAsync();
    Task<string?> GetAccessTokenAsync();
    bool IsAuthenticated { get; }
    string? UserEmail { get; }
    Guid? UserId { get; }
}
