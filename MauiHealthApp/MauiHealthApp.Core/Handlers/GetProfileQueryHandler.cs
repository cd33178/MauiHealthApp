using MauiHealthApp.Core.Queries;
using MauiHealthApp.Core.Services;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MediatR;

namespace MauiHealthApp.Core.Handlers;

public class GetProfileQueryHandler :
    IRequestHandler<GetProfileQuery, Result<ProfileDto>>,
    IRequestHandler<GetProfileByUserIdQuery, Result<ProfileDto>>
{
    private readonly IProfileService _profileService;

    public GetProfileQueryHandler(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<Result<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        => _profileService.GetProfileAsync(request.ProfileId);

    public Task<Result<ProfileDto>> Handle(GetProfileByUserIdQuery request, CancellationToken cancellationToken)
        => _profileService.GetProfileByUserIdAsync(request.UserId);
}
