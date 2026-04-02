using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Requests;

namespace MauiHealthApp.DataAccess.Repositories;

public interface IApiQuestionRepository
{
    Task<(List<QuestionDto> Items, int Total)> GetPagedAsync(Guid userId, int page, int pageSize, string? search);
    Task<QuestionDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateQuestionRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateQuestionRequest request);
    Task<bool> DeleteAsync(Guid id);
}
