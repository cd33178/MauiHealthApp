using Dapper;
using HealthApi.DataAccess.Contexts;
using HealthApi.Shared.Models;

namespace HealthApi.DataAccess.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly DapperContext _context;

    public ProfileRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM Profiles WHERE Id = @Id";
        var profile = await connection.QueryFirstOrDefaultAsync<Profile>(sql, new { Id = id });
        if (profile != null)
        {
            profile.MedicalConditions = (await GetMedicalConditionsAsync(profile.Id)).ToList();
        }
        return profile;
    }

    public async Task<Profile?> GetByUserIdAsync(Guid userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM Profiles WHERE UserId = @UserId";
        var profile = await connection.QueryFirstOrDefaultAsync<Profile>(sql, new { UserId = userId });
        if (profile != null)
        {
            profile.MedicalConditions = (await GetMedicalConditionsAsync(profile.Id)).ToList();
        }
        return profile;
    }

    public async Task<IEnumerable<Profile>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM Profiles ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Profile>(sql);
    }

    public async Task<Guid> CreateAsync(Profile profile)
    {
        using var connection = _context.CreateConnection();
        profile.Id = Guid.NewGuid();
        profile.CreatedAt = DateTime.UtcNow;
        const string sql = @"
            INSERT INTO Profiles (Id, UserId, DateOfBirth, WeightKg, HeightCm, BloodType, Notes, CreatedAt)
            VALUES (@Id, @UserId, @DateOfBirth, @WeightKg, @HeightCm, @BloodType, @Notes, @CreatedAt)";
        await connection.ExecuteAsync(sql, profile);
        return profile.Id;
    }

    public async Task<bool> UpdateAsync(Profile profile)
    {
        using var connection = _context.CreateConnection();
        profile.UpdatedAt = DateTime.UtcNow;
        const string sql = @"
            UPDATE Profiles SET
                DateOfBirth = @DateOfBirth,
                WeightKg = @WeightKg,
                HeightCm = @HeightCm,
                BloodType = @BloodType,
                Notes = @Notes,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, profile);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "DELETE FROM Profiles WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }

    public async Task<IEnumerable<MedicalCondition>> GetMedicalConditionsAsync(Guid profileId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM MedicalConditions WHERE ProfileId = @ProfileId ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<MedicalCondition>(sql, new { ProfileId = profileId });
    }

    public async Task<Guid> AddMedicalConditionAsync(MedicalCondition condition)
    {
        using var connection = _context.CreateConnection();
        condition.Id = Guid.NewGuid();
        condition.CreatedAt = DateTime.UtcNow;
        const string sql = @"
            INSERT INTO MedicalConditions (Id, ProfileId, Name, DiagnosedAt, Severity, Notes, CreatedAt)
            VALUES (@Id, @ProfileId, @Name, @DiagnosedAt, @Severity, @Notes, @CreatedAt)";
        await connection.ExecuteAsync(sql, condition);
        return condition.Id;
    }

    public async Task<bool> DeleteMedicalConditionAsync(Guid conditionId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "DELETE FROM MedicalConditions WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { Id = conditionId });
        return rows > 0;
    }
}
