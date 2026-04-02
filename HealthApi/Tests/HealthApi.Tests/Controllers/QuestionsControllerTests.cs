using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthApi.Controllers;
using HealthApi.DataAccess.Repositories;
using HealthApi.Shared.Models;
using HealthApi.Shared.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HealthApi.Tests.Controllers;

public class QuestionsControllerTests
{
    private readonly Mock<IQuestionRepository> _questionRepoMock = new();
    private readonly Mock<IValidator<CreateQuestionRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateQuestionRequest>> _updateValidatorMock = new();
    private readonly Mock<ILogger<QuestionsController>> _loggerMock = new();

    private QuestionsController CreateController() => new(
        _questionRepoMock.Object,
        _createValidatorMock.Object,
        _updateValidatorMock.Object,
        _loggerMock.Object);

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _questionRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Question { Id = id, UserId = Guid.NewGuid(), QuestionText = "Test?", CreatedAt = DateTime.UtcNow });

        var result = await CreateController().GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        _questionRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Question?)null);

        var result = await CreateController().GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetPaged_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _questionRepoMock.Setup(r => r.GetPagedAsync(userId, 1, 10, null))
            .ReturnsAsync((new List<Question>(), 0));

        var result = await CreateController().GetPaged(userId, 1, 10, null);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateQuestionRequest { UserId = Guid.NewGuid(), QuestionText = "Test?" };
        var newId = Guid.NewGuid();
        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _questionRepoMock.Setup(r => r.CreateAsync(It.IsAny<Question>())).ReturnsAsync(newId);
        _questionRepoMock.Setup(r => r.GetByIdAsync(newId))
            .ReturnsAsync(new Question { Id = newId, UserId = request.UserId, QuestionText = request.QuestionText, CreatedAt = DateTime.UtcNow });

        var result = await CreateController().Create(request);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }
}
