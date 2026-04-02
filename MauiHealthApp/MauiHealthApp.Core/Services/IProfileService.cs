using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;

namespace MauiHealthApp.Core.Services;

public interface IProfileService
{
    Task<Result<ProfileDto>> GetProfileAsync(Guid id);
    Task<Result<ProfileDto>> GetProfileByUserIdAsync(Guid userId);
    Task<Result<Guid>> CreateProfileAsync(CreateProfileRequest request);
    Task<Result<ProfileDto>> UpdateProfileAsync(Guid id, UpdateProfileRequest request);
    Task<Result> DeleteProfileAsync(Guid id);
}
