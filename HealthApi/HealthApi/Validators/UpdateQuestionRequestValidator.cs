using FluentValidation;
using HealthApi.Shared.Requests;

namespace HealthApi.Validators;

public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(x => x.AnswerText)
            .MaximumLength(5000).When(x => !string.IsNullOrEmpty(x.AnswerText))
            .WithMessage("Answer text cannot exceed 5000 characters");
        RuleFor(x => x.Category)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Category))
            .WithMessage("Category cannot exceed 100 characters");
    }
}
