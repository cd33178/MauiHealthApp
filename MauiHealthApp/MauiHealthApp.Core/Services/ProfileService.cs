using MauiHealthApp.Core.Services;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using Microsoft.Extensions.Logging;

namespace MauiHealthApp.Core.Services;

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly Func<Guid, Task<ProfileDto?>> _getFromApi;
    private readonly Func<Guid, Task<ProfileDto?>> _getByUserIdFromApi;
    private readonly Func<CreateProfileRequest, Task<Guid>> _createInApi;
    private readonly Func<Guid, UpdateProfileRequest, Task<bool>> _updateInApi;
    private readonly Func<Guid, Task<bool>> _deleteFromApi;

    public ProfileService(ILogger<ProfileService> logger,
        Func<Guid, Task<ProfileDto?>> getFromApi,
        Func<Guid, Task<ProfileDto?>> getByUserIdFromApi,
        Func<CreateProfileRequest, Task<Guid>> createInApi,
        Func<Guid, UpdateProfileRequest, Task<bool>> updateInApi,
        Func<Guid, Task<bool>> deleteFromApi)
    {
        _logger = logger;
        _getFromApi = getFromApi;
        _getByUserIdFromApi = getByUserIdFromApi;
        _createInApi = createInApi;
        _updateInApi = updateInApi;
        _deleteFromApi = deleteFromApi;
    }

    public async Task<Result<ProfileDto>> GetProfileAsync(Guid id)
    {
        try
        {
            var profile = await _getFromApi(id);
            return profile != null
                ? Result<ProfileDto>.Success(profile)
                : Result<ProfileDto>.Failure("Profile not found");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error getting profile {Id}", id);
            return Result<ProfileDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile {Id}", id);
            return Result<ProfileDto>.Failure("An error occurred while retrieving the profile");
        }
    }

    public async Task<Result<ProfileDto>> GetProfileByUserIdAsync(Guid userId)
    {
        try
        {
            var profile = await _getByUserIdFromApi(userId);
            return profile != null
                ? Result<ProfileDto>.Success(profile)
                : Result<ProfileDto>.Failure("Profile not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
            return Result<ProfileDto>.Failure("An error occurred while retrieving the profile");
        }
    }

    public async Task<Result<Guid>> CreateProfileAsync(CreateProfileRequest request)
    {
        try
        {
            var id = await _createInApi(request);
            return Result<Guid>.Success(id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error creating profile");
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile");
            return Result<Guid>.Failure("An error occurred while creating the profile");
        }
    }

    public async Task<Result<ProfileDto>> UpdateProfileAsync(Guid id, UpdateProfileRequest request)
    {
        try
        {
            await _updateInApi(id, request);
            var updated = await _getFromApi(id);
            return updated != null
                ? Result<ProfileDto>.Success(updated)
                : Result<ProfileDto>.Failure("Profile not found after update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile {Id}", id);
            return Result<ProfileDto>.Failure("An error occurred while updating the profile");
        }
    }

    public async Task<Result> DeleteProfileAsync(Guid id)
    {
        try
        {
            var deleted = await _deleteFromApi(id);
            return deleted ? Result.Success() : Result.Failure("Profile not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile {Id}", id);
            return Result.Failure("An error occurred while deleting the profile");
        }
    }
}
