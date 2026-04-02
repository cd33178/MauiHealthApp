using MauiHealthApp.DataAccess.Contexts;

namespace MauiHealthApp.DataAccess.Sqlite;

public class DatabaseInitializer
{
    private readonly LocalDbContext _context;

    public DatabaseInitializer(LocalDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        await _context.GetConnectionAsync();
    }
}
