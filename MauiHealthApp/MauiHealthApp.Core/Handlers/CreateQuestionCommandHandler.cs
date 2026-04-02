using MauiHealthApp.Core.Commands;
using MauiHealthApp.Core.Services;
using MauiHealthApp.Core.Validators;
using MauiHealthApp.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MauiHealthApp.Core.Handlers;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<Guid>>
{
    private readonly IQuestionService _questionService;
    private readonly CreateQuestionCommandValidator _validator;
    private readonly ILogger<CreateQuestionCommandHandler> _logger;

    public CreateQuestionCommandHandler(
        IQuestionService questionService,
        CreateQuestionCommandValidator validator,
        ILogger<CreateQuestionCommandHandler> logger)
    {
        _questionService = questionService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<Guid>.Failure(errors);
        }

        return await _questionService.CreateQuestionAsync(request.Request);
    }
}
