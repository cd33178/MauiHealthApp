using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MediatR;

namespace MauiHealthApp.Core.Queries;

public record GetQuestionsQuery(Guid UserId, int Page = 1, int PageSize = 10, string? Search = null)
    : IRequest<Result<PagedResult<QuestionDto>>>;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
