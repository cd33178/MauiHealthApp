namespace HealthApi.Shared.Requests;
public class UpdateQuestionRequest
{
    public string? AnswerText { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool? IsAnswered { get; set; }
}
