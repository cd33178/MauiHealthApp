using MauiHealthApp.Core.Queries;
using MauiHealthApp.Core.Services;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MediatR;

namespace MauiHealthApp.Core.Handlers;

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, Result<PagedResult<QuestionDto>>>
{
    private readonly IQuestionService _questionService;

    public GetQuestionsQueryHandler(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public Task<Result<PagedResult<QuestionDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
        => _questionService.GetQuestionsAsync(request.UserId, request.Page, request.PageSize, request.Search);
}
