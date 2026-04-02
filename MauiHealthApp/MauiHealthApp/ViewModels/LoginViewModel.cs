using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHealthApp.Services;

namespace MauiHealthApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Health App Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        await ExecuteAsync(async () =>
        {
            var success = await _authService.LoginAsync();
            if (success)
                await _navigationService.NavigateToAsync("//profile");
            else
                ErrorMessage = "Login failed. Please try again.";
        });
    }
}
