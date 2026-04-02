using MauiHealthApp.DataAccess.Contexts;

namespace MauiHealthApp.DataAccess.Repositories;

public interface ILocalProfileRepository
{
    Task<LocalProfile?> GetByIdAsync(Guid id);
    Task<LocalProfile?> GetByUserIdAsync(Guid userId);
    Task SaveAsync(LocalProfile profile);
    Task DeleteAsync(Guid id);
}
