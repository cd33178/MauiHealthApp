using FluentValidation;
using MauiHealthApp.Core.Commands;

namespace MauiHealthApp.Core.Validators;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Request.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MaximumLength(2000).WithMessage("Question text cannot exceed 2000 characters");
    }
}
