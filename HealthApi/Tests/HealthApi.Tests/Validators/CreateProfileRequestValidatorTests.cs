using FluentAssertions;
using HealthApi.Validators;
using HealthApi.Shared.Requests;
using Xunit;

namespace HealthApi.Tests.Validators;

public class CreateProfileRequestValidatorTests
{
    private readonly CreateProfileRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_Passes()
    {
        var request = new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            DateOfBirth = new DateTime(1990, 1, 1),
            WeightKg = 70,
            HeightCm = 175,
            BloodType = "A+"
        };

        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_Fails()
    {
        var request = new CreateProfileRequest { UserId = Guid.Empty };
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public async Task Validate_WithFutureDateOfBirth_Fails()
    {
        var request = new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            DateOfBirth = DateTime.Today.AddDays(1)
        };
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithInvalidBloodType_Fails()
    {
        var request = new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            BloodType = "INVALID"
        };
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithWeightOutOfRange_Fails()
    {
        var request = new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            WeightKg = 1000
        };
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
    }
}
