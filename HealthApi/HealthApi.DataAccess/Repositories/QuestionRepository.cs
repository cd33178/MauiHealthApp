using Dapper;
using HealthApi.DataAccess.Contexts;
using HealthApi.Shared.Models;

namespace HealthApi.DataAccess.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly DapperContext _context;

    public QuestionRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM Questions WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Question>(sql, new { Id = id });
    }

    public async Task<(IEnumerable<Question> Items, int TotalCount)> GetPagedAsync(
        Guid userId, int page, int pageSize, string? search)
    {
        using var connection = _context.CreateConnection();
        var offset = (page - 1) * pageSize;
        var whereClause = "WHERE UserId = @UserId";
        if (!string.IsNullOrWhiteSpace(search))
            whereClause += " AND (QuestionText LIKE @Search OR AnswerText LIKE @Search OR Category LIKE @Search OR Tags LIKE @Search)";

        var countSql = $"SELECT COUNT(*) FROM Questions {whereClause}";
        var dataSql = $@"
            SELECT * FROM Questions {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new
        {
            UserId = userId,
            Search = $"%{search}%",
            Offset = offset,
            PageSize = pageSize
        };

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<Question>(dataSql, parameters);
        return (items, totalCount);
    }

    public async Task<IEnumerable<Question>> GetByUserIdAsync(Guid userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM Questions WHERE UserId = @UserId ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Question>(sql, new { UserId = userId });
    }

    public async Task<Guid> CreateAsync(Question question)
    {
        using var connection = _context.CreateConnection();
        question.Id = Guid.NewGuid();
        question.CreatedAt = DateTime.UtcNow;
        const string sql = @"
            INSERT INTO Questions (Id, UserId, QuestionText, AnswerText, Category, Tags, IsAnswered, CreatedAt)
            VALUES (@Id, @UserId, @QuestionText, @AnswerText, @Category, @Tags, @IsAnswered, @CreatedAt)";
        await connection.ExecuteAsync(sql, question);
        return question.Id;
    }

    public async Task<bool> UpdateAsync(Question question)
    {
        using var connection = _context.CreateConnection();
        question.UpdatedAt = DateTime.UtcNow;
        const string sql = @"
            UPDATE Questions SET
                AnswerText = @AnswerText,
                Category = @Category,
                Tags = @Tags,
                IsAnswered = @IsAnswered,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, question);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "DELETE FROM Questions WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
