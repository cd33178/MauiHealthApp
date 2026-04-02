namespace MauiHealthApp.Shared.DTOs;

public class MedicalConditionDto
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DiagnosedAt { get; set; }
    public string? Severity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
