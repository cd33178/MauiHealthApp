using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using MediatR;

namespace MauiHealthApp.Core.Commands;

public record CreateProfileCommand(CreateProfileRequest Request) : IRequest<Result<Guid>>;
