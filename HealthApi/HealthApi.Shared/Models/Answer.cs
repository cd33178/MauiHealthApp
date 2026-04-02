namespace HealthApi.Shared.Models;
public class Answer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public string? Source { get; set; }
    public decimal? Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
