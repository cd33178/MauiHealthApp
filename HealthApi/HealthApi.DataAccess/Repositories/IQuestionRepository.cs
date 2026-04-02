using HealthApi.Shared.Models;

namespace HealthApi.DataAccess.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Question> Items, int TotalCount)> GetPagedAsync(Guid userId, int page, int pageSize, string? search);
    Task<IEnumerable<Question>> GetByUserIdAsync(Guid userId);
    Task<Guid> CreateAsync(Question question);
    Task<bool> UpdateAsync(Question question);
    Task<bool> DeleteAsync(Guid id);
}
