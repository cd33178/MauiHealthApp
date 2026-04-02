using MauiHealthApp.ViewModels;

namespace MauiHealthApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
            vm.LoadProfileCommand.Execute(null);
    }
}
