namespace MauiHealthApp.Shared.Models;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string? ErrorCode { get; }
    public List<string> Errors { get; } = new();

    public ApiException(int statusCode, string message, string? errorCode = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public ApiException(int statusCode, string message, List<string> errors)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public bool IsUnauthorized => StatusCode == 401;
    public bool IsForbidden => StatusCode == 403;
    public bool IsNotFound => StatusCode == 404;
    public bool IsServerError => StatusCode >= 500;
}
