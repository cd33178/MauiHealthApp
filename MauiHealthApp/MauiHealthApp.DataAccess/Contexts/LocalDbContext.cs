using SQLite;

namespace MauiHealthApp.DataAccess.Contexts;

public class LocalDbContext
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public LocalDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await InitializeAsync();
        }
        return _database;
    }

    private async Task InitializeAsync()
    {
        if (_database == null) return;
        await _database.CreateTableAsync<LocalProfile>();
        await _database.CreateTableAsync<LocalQuestion>();
    }
}

[Table("Profiles")]
public class LocalProfile
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public string? BloodType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime CachedAt { get; set; }
}

[Table("Questions")]
public class LocalQuestion
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public string? Category { get; set; }
    public bool IsAnswered { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime CachedAt { get; set; }
}
