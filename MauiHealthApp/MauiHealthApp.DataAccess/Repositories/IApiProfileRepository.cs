using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Requests;

namespace MauiHealthApp.DataAccess.Repositories;

public interface IApiProfileRepository
{
    Task<ProfileDto?> GetByIdAsync(Guid id);
    Task<ProfileDto?> GetByUserIdAsync(Guid userId);
    Task<Guid> CreateAsync(CreateProfileRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateProfileRequest request);
    Task<bool> DeleteAsync(Guid id);
}
