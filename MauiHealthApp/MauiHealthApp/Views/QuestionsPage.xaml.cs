using MauiHealthApp.ViewModels;

namespace MauiHealthApp.Views;

public partial class QuestionsPage : ContentPage
{
    public QuestionsPage(QuestionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is QuestionsViewModel vm)
            vm.LoadQuestionsCommand.Execute(null);
    }
}
