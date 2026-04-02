using FluentValidation;
using MauiHealthApp.Core.Commands;

namespace MauiHealthApp.Core.Validators;

public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidator()
    {
        RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Request.DateOfBirth)
            .LessThan(DateTime.UtcNow.Date).When(x => x.Request.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be in the past");
        RuleFor(x => x.Request.WeightKg)
            .InclusiveBetween(1, 500).When(x => x.Request.WeightKg.HasValue)
            .WithMessage("Weight must be between 1 and 500 kg");
        RuleFor(x => x.Request.HeightCm)
            .InclusiveBetween(1, 300).When(x => x.Request.HeightCm.HasValue)
            .WithMessage("Height must be between 1 and 300 cm");
        RuleFor(x => x.Request.BloodType)
            .Matches(@"^(A|B|AB|O)[+-]$").When(x => !string.IsNullOrEmpty(x.Request.BloodType))
            .WithMessage("Invalid blood type");
    }
}
