namespace HealthApi.Shared.Models;
public class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public string? BloodType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MedicalCondition> MedicalConditions { get; set; } = new();
}
