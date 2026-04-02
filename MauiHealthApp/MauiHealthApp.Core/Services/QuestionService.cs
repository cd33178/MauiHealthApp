using MauiHealthApp.Core.Queries;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using Microsoft.Extensions.Logging;

namespace MauiHealthApp.Core.Services;

public class QuestionService : IQuestionService
{
    private readonly ILogger<QuestionService> _logger;
    private readonly Func<Guid, int, int, string?, Task<(List<QuestionDto> Items, int Total)>> _getPagedFromApi;
    private readonly Func<Guid, Task<QuestionDto?>> _getFromApi;
    private readonly Func<CreateQuestionRequest, Task<Guid>> _createInApi;
    private readonly Func<Guid, UpdateQuestionRequest, Task<bool>> _updateInApi;
    private readonly Func<Guid, Task<bool>> _deleteFromApi;

    public QuestionService(ILogger<QuestionService> logger,
        Func<Guid, int, int, string?, Task<(List<QuestionDto> Items, int Total)>> getPagedFromApi,
        Func<Guid, Task<QuestionDto?>> getFromApi,
        Func<CreateQuestionRequest, Task<Guid>> createInApi,
        Func<Guid, UpdateQuestionRequest, Task<bool>> updateInApi,
        Func<Guid, Task<bool>> deleteFromApi)
    {
        _logger = logger;
        _getPagedFromApi = getPagedFromApi;
        _getFromApi = getFromApi;
        _createInApi = createInApi;
        _updateInApi = updateInApi;
        _deleteFromApi = deleteFromApi;
    }

    public async Task<Result<PagedResult<QuestionDto>>> GetQuestionsAsync(Guid userId, int page, int pageSize, string? search)
    {
        try
        {
            var (items, total) = await _getPagedFromApi(userId, page, pageSize, search);
            return Result<PagedResult<QuestionDto>>.Success(new PagedResult<QuestionDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions for user {UserId}", userId);
            return Result<PagedResult<QuestionDto>>.Failure("Error retrieving questions");
        }
    }

    public async Task<Result<QuestionDto>> GetQuestionAsync(Guid id)
    {
        try
        {
            var question = await _getFromApi(id);
            return question != null
                ? Result<QuestionDto>.Success(question)
                : Result<QuestionDto>.Failure("Question not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question {Id}", id);
            return Result<QuestionDto>.Failure("Error retrieving question");
        }
    }

    public async Task<Result<Guid>> CreateQuestionAsync(CreateQuestionRequest request)
    {
        try
        {
            var id = await _createInApi(request);
            return Result<Guid>.Success(id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error creating question");
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return Result<Guid>.Failure("Error creating question");
        }
    }

    public async Task<Result<QuestionDto>> UpdateQuestionAsync(Guid id, UpdateQuestionRequest request)
    {
        try
        {
            await _updateInApi(id, request);
            var updated = await _getFromApi(id);
            return updated != null
                ? Result<QuestionDto>.Success(updated)
                : Result<QuestionDto>.Failure("Question not found after update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question {Id}", id);
            return Result<QuestionDto>.Failure("Error updating question");
        }
    }

    public async Task<Result> DeleteQuestionAsync(Guid id)
    {
        try
        {
            var deleted = await _deleteFromApi(id);
            return deleted ? Result.Success() : Result.Failure("Question not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {Id}", id);
            return Result.Failure("Error deleting question");
        }
    }
}
