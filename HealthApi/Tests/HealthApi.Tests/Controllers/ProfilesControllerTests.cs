using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthApi.Controllers;
using HealthApi.DataAccess.Repositories;
using HealthApi.Shared.DTOs;
using HealthApi.Shared.Models;
using HealthApi.Shared.Requests;
using HealthApi.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HealthApi.Tests.Controllers;

public class ProfilesControllerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IValidator<CreateProfileRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateProfileRequest>> _updateValidatorMock = new();
    private readonly Mock<ILogger<ProfilesController>> _loggerMock = new();

    private ProfilesController CreateController() => new(
        _profileRepoMock.Object,
        _createValidatorMock.Object,
        _updateValidatorMock.Object,
        _loggerMock.Object);

    [Fact]
    public async Task GetById_WhenProfileExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _profileRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Profile { Id = id, UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });

        var result = await CreateController().GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenProfileNotFound_ReturnsNotFound()
    {
        _profileRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Profile?)null);

        var result = await CreateController().GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateProfileRequest { UserId = Guid.NewGuid(), WeightKg = 70 };
        var newId = Guid.NewGuid();
        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _profileRepoMock.Setup(r => r.CreateAsync(It.IsAny<Profile>())).ReturnsAsync(newId);
        _profileRepoMock.Setup(r => r.GetByIdAsync(newId))
            .ReturnsAsync(new Profile { Id = newId, UserId = request.UserId, CreatedAt = DateTime.UtcNow });

        var result = await CreateController().Create(request);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new CreateProfileRequest();
        var validationResult = new ValidationResult(new[] { new ValidationFailure("UserId", "Required") });
        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        var result = await CreateController().Create(request);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_WhenProfileExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _profileRepoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        var result = await CreateController().Delete(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
