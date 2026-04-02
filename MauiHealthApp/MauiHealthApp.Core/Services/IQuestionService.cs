using MauiHealthApp.Core.Queries;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;

namespace MauiHealthApp.Core.Services;

public interface IQuestionService
{
    Task<Result<PagedResult<QuestionDto>>> GetQuestionsAsync(Guid userId, int page, int pageSize, string? search);
    Task<Result<QuestionDto>> GetQuestionAsync(Guid id);
    Task<Result<Guid>> CreateQuestionAsync(CreateQuestionRequest request);
    Task<Result<QuestionDto>> UpdateQuestionAsync(Guid id, UpdateQuestionRequest request);
    Task<Result> DeleteQuestionAsync(Guid id);
}
