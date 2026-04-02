using HealthApi.Shared.Models;

namespace HealthApi.DataAccess.Repositories;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id);
    Task<Profile?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Profile>> GetAllAsync();
    Task<Guid> CreateAsync(Profile profile);
    Task<bool> UpdateAsync(Profile profile);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<MedicalCondition>> GetMedicalConditionsAsync(Guid profileId);
    Task<Guid> AddMedicalConditionAsync(MedicalCondition condition);
    Task<bool> DeleteMedicalConditionAsync(Guid conditionId);
}
