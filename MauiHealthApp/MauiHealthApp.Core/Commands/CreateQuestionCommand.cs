using MauiHealthApp.Shared.Models;
using MauiHealthApp.Shared.Requests;
using MediatR;

namespace MauiHealthApp.Core.Commands;

public record CreateQuestionCommand(CreateQuestionRequest Request) : IRequest<Result<Guid>>;
