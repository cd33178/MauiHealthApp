using MauiHealthApp.DataAccess.Contexts;
using Microsoft.Extensions.Logging;

namespace MauiHealthApp.DataAccess.Repositories;

public class LocalProfileRepository : ILocalProfileRepository
{
    private readonly LocalDbContext _context;
    private readonly ILogger<LocalProfileRepository> _logger;

    public LocalProfileRepository(LocalDbContext context, ILogger<LocalProfileRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LocalProfile?> GetByIdAsync(Guid id)
    {
        var db = await _context.GetConnectionAsync();
        return await db.Table<LocalProfile>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<LocalProfile?> GetByUserIdAsync(Guid userId)
    {
        var db = await _context.GetConnectionAsync();
        return await db.Table<LocalProfile>().Where(p => p.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(LocalProfile profile)
    {
        var db = await _context.GetConnectionAsync();
        profile.CachedAt = DateTime.UtcNow;
        var existing = await db.Table<LocalProfile>().Where(p => p.Id == profile.Id).FirstOrDefaultAsync();
        if (existing == null)
            await db.InsertAsync(profile);
        else
            await db.UpdateAsync(profile);
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = await _context.GetConnectionAsync();
        await db.DeleteAsync<LocalProfile>(id);
    }
}
