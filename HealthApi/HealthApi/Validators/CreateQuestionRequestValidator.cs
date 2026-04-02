using FluentValidation;
using HealthApi.Shared.Requests;

namespace HealthApi.Validators;

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MaximumLength(2000).WithMessage("Question text cannot exceed 2000 characters");
        RuleFor(x => x.Category)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Category))
            .WithMessage("Category cannot exceed 100 characters");
    }
}
