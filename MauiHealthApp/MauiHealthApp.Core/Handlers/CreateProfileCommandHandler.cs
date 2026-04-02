using MauiHealthApp.Core.Commands;
using MauiHealthApp.Core.Services;
using MauiHealthApp.Core.Validators;
using MauiHealthApp.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MauiHealthApp.Core.Handlers;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Result<Guid>>
{
    private readonly IProfileService _profileService;
    private readonly CreateProfileCommandValidator _validator;
    private readonly ILogger<CreateProfileCommandHandler> _logger;

    public CreateProfileCommandHandler(
        IProfileService profileService,
        CreateProfileCommandValidator validator,
        ILogger<CreateProfileCommandHandler> logger)
    {
        _profileService = profileService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation failed for CreateProfileCommand: {Errors}", string.Join(", ", errors));
            return Result<Guid>.Failure(errors);
        }

        return await _profileService.CreateProfileAsync(request.Request);
    }
}
