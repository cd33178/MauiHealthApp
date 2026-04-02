namespace HealthApi.Shared.DTOs;
public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool IsAnswered { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
