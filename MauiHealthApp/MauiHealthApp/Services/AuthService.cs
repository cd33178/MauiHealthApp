using Microsoft.Identity.Client;

namespace MauiHealthApp.Services;

public class AuthService : IAuthService
{
    private readonly IPublicClientApplication _msalClient;
    private readonly string[] _scopes;
    private AuthenticationResult? _authResult;

    public AuthService(IPublicClientApplication msalClient, IConfiguration configuration)
    {
        _msalClient = msalClient;
        _scopes = configuration.GetSection("MsalSettings:Scopes").Get<string[]>()
            ?? new[] { "openid", "profile", "email" };
    }

    public bool IsAuthenticated => _authResult != null && _authResult.ExpiresOn > DateTimeOffset.UtcNow;
    public string? UserEmail => _authResult?.Account?.Username;
    public Guid? UserId => _authResult?.UniqueId is string uid && Guid.TryParse(uid, out var g) ? g : null;

    public async Task<bool> LoginAsync()
    {
        try
        {
            var accounts = await _msalClient.GetAccountsAsync();
            _authResult = await _msalClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
            return true;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                _authResult = await _msalClient.AcquireTokenInteractive(_scopes).ExecuteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        var accounts = await _msalClient.GetAccountsAsync();
        foreach (var account in accounts)
            await _msalClient.RemoveAsync(account);
        _authResult = null;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (_authResult == null || _authResult.ExpiresOn <= DateTimeOffset.UtcNow.AddMinutes(5))
            await LoginAsync();
        return _authResult?.AccessToken;
    }
}
