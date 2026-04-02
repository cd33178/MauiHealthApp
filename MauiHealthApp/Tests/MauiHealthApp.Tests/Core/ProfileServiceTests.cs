using FluentAssertions;
using MauiHealthApp.Core.Services;
using Xunit;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using Microsoft.Extensions.Logging;
using Moq;

namespace MauiHealthApp.Tests.Core;

public class ProfileServiceTests
{
    private readonly Mock<ILogger<ProfileService>> _loggerMock = new();

    private ProfileService CreateService(
        Func<Guid, Task<ProfileDto?>>? getFromApi = null,
        Func<Guid, Task<ProfileDto?>>? getByUserIdFromApi = null,
        Func<CreateProfileRequest, Task<Guid>>? createInApi = null,
        Func<Guid, UpdateProfileRequest, Task<bool>>? updateInApi = null,
        Func<Guid, Task<bool>>? deleteFromApi = null)
    {
        return new ProfileService(
            _loggerMock.Object,
            getFromApi ?? (_ => Task.FromResult<ProfileDto?>(null)),
            getByUserIdFromApi ?? (_ => Task.FromResult<ProfileDto?>(null)),
            createInApi ?? (_ => Task.FromResult(Guid.NewGuid())),
            updateInApi ?? ((_, _) => Task.FromResult(true)),
            deleteFromApi ?? (_ => Task.FromResult(true)));
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfileExists_ReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var dto = new ProfileDto { Id = id, UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var service = CreateService(getFromApi: _ => Task.FromResult<ProfileDto?>(dto));

        var result = await service.GetProfileAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfileNotFound_ReturnsFailure()
    {
        var service = CreateService(getFromApi: _ => Task.FromResult<ProfileDto?>(null));

        var result = await service.GetProfileAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateProfileAsync_WithValidRequest_ReturnsSuccess()
    {
        var expectedId = Guid.NewGuid();
        var service = CreateService(createInApi: _ => Task.FromResult(expectedId));

        var result = await service.CreateProfileAsync(new CreateProfileRequest { UserId = Guid.NewGuid() });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task CreateProfileAsync_WhenApiThrows_ReturnsFailure()
    {
        var service = CreateService(createInApi: _ => throw new HttpRequestException("Connection refused"));

        var result = await service.CreateProfileAsync(new CreateProfileRequest { UserId = Guid.NewGuid() });

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteProfileAsync_WhenSuccessful_ReturnsSuccess()
    {
        var service = CreateService(deleteFromApi: _ => Task.FromResult(true));

        var result = await service.DeleteProfileAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
    }
}
