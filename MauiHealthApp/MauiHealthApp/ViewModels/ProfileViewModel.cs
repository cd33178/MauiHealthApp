using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHealthApp.Core.Commands;
using MauiHealthApp.Core.Queries;
using MauiHealthApp.Services;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Requests;
using MediatR;

namespace MauiHealthApp.ViewModels;

[QueryProperty(nameof(ProfileId), "profileId")]
public partial class ProfileViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private string? _profileId;
    [ObservableProperty] private Guid _id;
    [ObservableProperty] private DateTime _dateOfBirth = DateTime.Today.AddYears(-30);
    [ObservableProperty] private decimal _weightKg;
    [ObservableProperty] private decimal _heightCm;
    [ObservableProperty] private string? _bloodType;
    [ObservableProperty] private string? _notes;
    [ObservableProperty] private List<MedicalConditionDto> _medicalConditions = new();
    [ObservableProperty] private bool _hasProfile;

    public ProfileViewModel(IMediator mediator, IAuthService authService, INavigationService navigationService)
    {
        _mediator = mediator;
        _authService = authService;
        _navigationService = navigationService;
        Title = "Health Profile";
    }

    partial void OnProfileIdChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out var id))
            _ = LoadProfileByIdAsync(id);
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        var userId = _authService.UserId;
        if (userId == null) return;

        await ExecuteAsync(async () =>
        {
            var result = await _mediator.Send(new GetProfileByUserIdQuery(userId.Value));
            if (result.IsSuccess && result.Value != null)
            {
                HasProfile = true;
                PopulateFromDto(result.Value);
            }
        });
    }

    private async Task LoadProfileByIdAsync(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            var result = await _mediator.Send(new GetProfileQuery(id));
            if (result.IsSuccess && result.Value != null)
            {
                HasProfile = true;
                PopulateFromDto(result.Value);
            }
        });
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        await ExecuteAsync(async () =>
        {
            var userId = _authService.UserId;
            if (userId == null)
            {
                ErrorMessage = "You must be logged in to save a profile.";
                return;
            }
            var request = new CreateProfileRequest
            {
                UserId = userId.Value,
                DateOfBirth = DateOfBirth,
                WeightKg = WeightKg,
                HeightCm = HeightCm,
                BloodType = BloodType,
                Notes = Notes
            };

            var result = await _mediator.Send(new CreateProfileCommand(request));
            if (result.IsSuccess)
            {
                HasProfile = true;
                await Application.Current!.MainPage!.DisplayAlert("Success", "Profile saved successfully!", "OK");
            }
            else
            {
                ErrorMessage = result.Error;
            }
        });
    }

    [RelayCommand]
    private async Task NavigateToQuestionsAsync()
    {
        await _navigationService.NavigateToAsync("//questions");
    }

    private void PopulateFromDto(ProfileDto dto)
    {
        Id = dto.Id;
        DateOfBirth = dto.DateOfBirth ?? DateTime.Today.AddYears(-30);
        WeightKg = dto.WeightKg ?? 0;
        HeightCm = dto.HeightCm ?? 0;
        BloodType = dto.BloodType;
        Notes = dto.Notes;
        MedicalConditions = dto.MedicalConditions;
    }
}
