using FluentValidation;
using HealthApi.DataAccess.Repositories;
using HealthApi.Shared.DTOs;
using HealthApi.Shared.Models;
using HealthApi.Shared.Requests;
using HealthApi.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IValidator<CreateProfileRequest> _createValidator;
    private readonly IValidator<UpdateProfileRequest> _updateValidator;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(
        IProfileRepository profileRepository,
        IValidator<CreateProfileRequest> createValidator,
        IValidator<UpdateProfileRequest> updateValidator,
        ILogger<ProfilesController> logger)
    {
        _profileRepository = profileRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProfileDto>>>> GetAll()
    {
        var profiles = await _profileRepository.GetAllAsync();
        var dtos = profiles.Select(MapToDto);
        return Ok(ApiResponse<IEnumerable<ProfileDto>>.Ok(dtos));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> GetById(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile == null)
            return NotFound(ApiResponse<ProfileDto>.Fail($"Profile {id} not found"));
        return Ok(ApiResponse<ProfileDto>.Ok(MapToDto(profile)));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> GetByUserId(Guid userId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile == null)
            return NotFound(ApiResponse<ProfileDto>.Fail($"Profile for user {userId} not found"));
        return Ok(ApiResponse<ProfileDto>.Ok(MapToDto(profile)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> Create([FromBody] CreateProfileRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<ProfileDto>.Fail("Validation failed", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var profile = new Profile
        {
            UserId = request.UserId,
            DateOfBirth = request.DateOfBirth,
            WeightKg = request.WeightKg,
            HeightCm = request.HeightCm,
            BloodType = request.BloodType,
            Notes = request.Notes
        };

        var id = await _profileRepository.CreateAsync(profile);
        var created = await _profileRepository.GetByIdAsync(id);
        _logger.LogInformation("Created profile {ProfileId} for user {UserId}", id, request.UserId);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<ProfileDto>.Ok(MapToDto(created!)));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> Update(Guid id, [FromBody] UpdateProfileRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<ProfileDto>.Fail("Validation failed", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var existing = await _profileRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<ProfileDto>.Fail($"Profile {id} not found"));

        existing.DateOfBirth = request.DateOfBirth ?? existing.DateOfBirth;
        existing.WeightKg = request.WeightKg ?? existing.WeightKg;
        existing.HeightCm = request.HeightCm ?? existing.HeightCm;
        existing.BloodType = request.BloodType ?? existing.BloodType;
        existing.Notes = request.Notes ?? existing.Notes;

        await _profileRepository.UpdateAsync(existing);
        _logger.LogInformation("Updated profile {ProfileId}", id);
        return Ok(ApiResponse<ProfileDto>.Ok(MapToDto(existing)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var deleted = await _profileRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Fail($"Profile {id} not found"));
        _logger.LogInformation("Deleted profile {ProfileId}", id);
        return Ok(ApiResponse<bool>.Ok(true, "Profile deleted successfully"));
    }

    [HttpPost("{id:guid}/conditions")]
    public async Task<ActionResult<ApiResponse<MedicalConditionDto>>> AddCondition(Guid id, [FromBody] MedicalConditionDto dto)
    {
        var condition = new MedicalCondition
        {
            ProfileId = id,
            Name = dto.Name,
            DiagnosedAt = dto.DiagnosedAt,
            Severity = dto.Severity,
            Notes = dto.Notes
        };
        var conditionId = await _profileRepository.AddMedicalConditionAsync(condition);
        condition.Id = conditionId;
        return Ok(ApiResponse<MedicalConditionDto>.Ok(MapConditionToDto(condition)));
    }

    [HttpDelete("{id:guid}/conditions/{conditionId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCondition(Guid id, Guid conditionId)
    {
        var deleted = await _profileRepository.DeleteMedicalConditionAsync(conditionId);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Fail($"Medical condition {conditionId} not found"));
        return Ok(ApiResponse<bool>.Ok(true));
    }

    private static ProfileDto MapToDto(Profile p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        DateOfBirth = p.DateOfBirth,
        WeightKg = p.WeightKg,
        HeightCm = p.HeightCm,
        BloodType = p.BloodType,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        MedicalConditions = p.MedicalConditions.Select(MapConditionToDto).ToList()
    };

    private static MedicalConditionDto MapConditionToDto(MedicalCondition c) => new()
    {
        Id = c.Id,
        ProfileId = c.ProfileId,
        Name = c.Name,
        DiagnosedAt = c.DiagnosedAt,
        Severity = c.Severity,
        Notes = c.Notes,
        CreatedAt = c.CreatedAt
    };
}
