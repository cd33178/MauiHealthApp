using FluentValidation;
using HealthApi.DataAccess.Repositories;
using HealthApi.Shared.DTOs;
using HealthApi.Shared.Models;
using HealthApi.Shared.Requests;
using HealthApi.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IValidator<CreateQuestionRequest> _createValidator;
    private readonly IValidator<UpdateQuestionRequest> _updateValidator;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(
        IQuestionRepository questionRepository,
        IValidator<CreateQuestionRequest> createValidator,
        IValidator<UpdateQuestionRequest> updateValidator,
        ILogger<QuestionsController> logger)
    {
        _questionRepository = questionRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<QuestionDto>>>> GetPaged(
        [FromQuery] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, total) = await _questionRepository.GetPagedAsync(userId, page, pageSize, search);
        var result = new PagedResponse<QuestionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return Ok(ApiResponse<PagedResponse<QuestionDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> GetById(Guid id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound(ApiResponse<QuestionDto>.Fail($"Question {id} not found"));
        return Ok(ApiResponse<QuestionDto>.Ok(MapToDto(question)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Create([FromBody] CreateQuestionRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<QuestionDto>.Fail("Validation failed", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var question = new Question
        {
            UserId = request.UserId,
            QuestionText = request.QuestionText,
            Category = request.Category,
            Tags = request.Tags,
            IsAnswered = false
        };

        var id = await _questionRepository.CreateAsync(question);
        var created = await _questionRepository.GetByIdAsync(id);
        _logger.LogInformation("Created question {QuestionId} for user {UserId}", id, request.UserId);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<QuestionDto>.Ok(MapToDto(created!)));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Update(Guid id, [FromBody] UpdateQuestionRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<QuestionDto>.Fail("Validation failed", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var existing = await _questionRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<QuestionDto>.Fail($"Question {id} not found"));

        existing.AnswerText = request.AnswerText ?? existing.AnswerText;
        existing.Category = request.Category ?? existing.Category;
        existing.Tags = request.Tags ?? existing.Tags;
        existing.IsAnswered = request.IsAnswered ?? existing.IsAnswered;

        await _questionRepository.UpdateAsync(existing);
        _logger.LogInformation("Updated question {QuestionId}", id);
        return Ok(ApiResponse<QuestionDto>.Ok(MapToDto(existing)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var deleted = await _questionRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Fail($"Question {id} not found"));
        _logger.LogInformation("Deleted question {QuestionId}", id);
        return Ok(ApiResponse<bool>.Ok(true, "Question deleted successfully"));
    }

    private static QuestionDto MapToDto(Question q) => new()
    {
        Id = q.Id,
        UserId = q.UserId,
        QuestionText = q.QuestionText,
        AnswerText = q.AnswerText,
        Category = q.Category,
        Tags = q.Tags,
        IsAnswered = q.IsAnswered,
        CreatedAt = q.CreatedAt,
        UpdatedAt = q.UpdatedAt
    };
}
