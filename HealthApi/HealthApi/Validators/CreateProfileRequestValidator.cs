using FluentValidation;
using HealthApi.Shared.Requests;

namespace HealthApi.Validators;

public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.Date).When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be in the past");
        RuleFor(x => x.WeightKg)
            .InclusiveBetween(1, 500).When(x => x.WeightKg.HasValue)
            .WithMessage("Weight must be between 1 and 500 kg");
        RuleFor(x => x.HeightCm)
            .InclusiveBetween(1, 300).When(x => x.HeightCm.HasValue)
            .WithMessage("Height must be between 1 and 300 cm");
        RuleFor(x => x.BloodType)
            .Matches(@"^(A|B|AB|O)[+-]$").When(x => !string.IsNullOrEmpty(x.BloodType))
            .WithMessage("Invalid blood type format");
    }
}
