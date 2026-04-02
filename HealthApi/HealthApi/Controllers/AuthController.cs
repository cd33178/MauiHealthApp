using HealthApi.Services;
using HealthApi.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> GetToken([FromBody] TokenRequest request)
    {
        var token = await _tokenService.GenerateTokenAsync(request.UserId, request.Email);
        if (token == null)
            return Unauthorized(ApiResponse<TokenResponse>.Fail("Invalid credentials"));
        return Ok(ApiResponse<TokenResponse>.Ok(token));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var token = await _tokenService.RefreshTokenAsync(request.RefreshToken);
        if (token == null)
            return Unauthorized(ApiResponse<TokenResponse>.Fail("Invalid or expired refresh token"));
        return Ok(ApiResponse<TokenResponse>.Ok(token));
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        await _tokenService.RevokeTokenAsync(request.RefreshToken);
        return Ok(ApiResponse<bool>.Ok(true, "Token revoked"));
    }
}

public class TokenRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
