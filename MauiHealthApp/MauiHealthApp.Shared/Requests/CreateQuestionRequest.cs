namespace MauiHealthApp.Shared.Requests;

public class CreateQuestionRequest
{
    public Guid UserId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
}
