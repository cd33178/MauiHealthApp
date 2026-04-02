using FluentAssertions;
using MauiHealthApp.Core.Commands;
using Xunit;
using MauiHealthApp.Core.Validators;
using MauiHealthApp.Shared.Requests;

namespace MauiHealthApp.Tests.Validators;

public class CreateProfileCommandValidatorTests
{
    private readonly CreateProfileCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidCommand_Passes()
    {
        var command = new CreateProfileCommand(new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            DateOfBirth = new DateTime(1990, 1, 1),
            WeightKg = 70,
            HeightCm = 175,
            BloodType = "A+"
        });

        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_Fails()
    {
        var command = new CreateProfileCommand(new CreateProfileRequest { UserId = Guid.Empty });

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("UserId"));
    }

    [Fact]
    public async Task Validate_WithFutureDateOfBirth_Fails()
    {
        var command = new CreateProfileCommand(new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            DateOfBirth = DateTime.Today.AddDays(1)
        });

        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithInvalidWeight_Fails()
    {
        var command = new CreateProfileCommand(new CreateProfileRequest
        {
            UserId = Guid.NewGuid(),
            WeightKg = 1000
        });

        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}
