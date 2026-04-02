using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Models;
using MediatR;

namespace MauiHealthApp.Core.Queries;

public record GetProfileQuery(Guid ProfileId) : IRequest<Result<ProfileDto>>;
public record GetProfileByUserIdQuery(Guid UserId) : IRequest<Result<ProfileDto>>;
