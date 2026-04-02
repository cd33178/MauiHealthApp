namespace MauiHealthApp.Shared.Requests;

public class UpdateProfileRequest
{
    public DateTime? DateOfBirth { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public string? BloodType { get; set; }
    public string? Notes { get; set; }
}
