using HealthApi.Shared.Responses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HealthApi.Services;

public interface ITokenService
{
    Task<TokenResponse?> GenerateTokenAsync(Guid userId, string email);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private static readonly Dictionary<string, (Guid UserId, string Email, DateTime Expiry)> _refreshTokens = new();

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<TokenResponse?> GenerateTokenAsync(Guid userId, string email)
    {
        var token = CreateToken(userId, email);
        return Task.FromResult<TokenResponse?>(token);
    }

    public Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        if (!_refreshTokens.TryGetValue(refreshToken, out var tokenInfo))
            return Task.FromResult<TokenResponse?>(null);

        if (tokenInfo.Expiry < DateTime.UtcNow)
        {
            _refreshTokens.Remove(refreshToken);
            return Task.FromResult<TokenResponse?>(null);
        }

        _refreshTokens.Remove(refreshToken);
        var newToken = CreateToken(tokenInfo.UserId, tokenInfo.Email);
        return Task.FromResult<TokenResponse?>(newToken);
    }

    public Task RevokeTokenAsync(string refreshToken)
    {
        _refreshTokens.Remove(refreshToken);
        return Task.CompletedTask;
    }

    private TokenResponse CreateToken(Guid userId, string email)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-please-change-in-production";
        var issuer = jwtSettings["Issuer"] ?? "HealthApi";
        var audience = jwtSettings["Audience"] ?? "HealthApp";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("scope", "health.read health.write"),
            new Claim(ClaimTypes.Role, "User")
        };

        var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: expiry, signingCredentials: credentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _refreshTokens[refreshToken] = (userId, email, DateTime.UtcNow.AddDays(7));

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiry,
            TokenType = "Bearer"
        };
    }
}
