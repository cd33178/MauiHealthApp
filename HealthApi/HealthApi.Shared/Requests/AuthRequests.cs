namespace HealthApi.Shared.Requests;

public class TokenRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
