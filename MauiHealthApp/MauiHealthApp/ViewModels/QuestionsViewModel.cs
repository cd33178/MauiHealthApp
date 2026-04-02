using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHealthApp.Core.Commands;
using MauiHealthApp.Core.Queries;
using MauiHealthApp.Services;
using MauiHealthApp.Shared.DTOs;
using MauiHealthApp.Shared.Requests;
using MediatR;
using System.Collections.ObjectModel;

namespace MauiHealthApp.ViewModels;

public partial class QuestionsViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IAuthService _authService;
    private int _currentPage = 1;
    private const int PageSize = 10;

    [ObservableProperty] private ObservableCollection<QuestionDto> _questions = new();
    [ObservableProperty] private string _newQuestionText = string.Empty;
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private bool _hasMorePages;
    [ObservableProperty] private int _totalCount;

    public QuestionsViewModel(IMediator mediator, IAuthService authService)
    {
        _mediator = mediator;
        _authService = authService;
        Title = "Health Q&A";
    }

    [RelayCommand]
    private async Task LoadQuestionsAsync()
    {
        var userId = _authService.UserId;
        if (userId == null) return;

        await ExecuteAsync(async () =>
        {
            _currentPage = 1;
            var result = await _mediator.Send(new GetQuestionsQuery(userId.Value, _currentPage, PageSize, SearchText));
            if (result.IsSuccess && result.Value != null)
            {
                Questions = new ObservableCollection<QuestionDto>(result.Value.Items);
                TotalCount = result.Value.TotalCount;
                HasMorePages = result.Value.TotalPages > _currentPage;
            }
        });
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (!HasMorePages) return;
        var userId = _authService.UserId;
        if (userId == null) return;

        await ExecuteAsync(async () =>
        {
            _currentPage++;
            var result = await _mediator.Send(new GetQuestionsQuery(userId.Value, _currentPage, PageSize, SearchText));
            if (result.IsSuccess && result.Value != null)
            {
                foreach (var q in result.Value.Items)
                    Questions.Add(q);
                HasMorePages = result.Value.TotalPages > _currentPage;
            }
        });
    }

    [RelayCommand]
    private async Task AskQuestionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewQuestionText)) return;
        var userId = _authService.UserId;
        if (userId == null) return;

        await ExecuteAsync(async () =>
        {
            var request = new CreateQuestionRequest
            {
                UserId = userId.Value,
                QuestionText = NewQuestionText
            };

            var result = await _mediator.Send(new CreateQuestionCommand(request));
            if (result.IsSuccess)
            {
                NewQuestionText = string.Empty;
                await LoadQuestionsAsync();
            }
            else
            {
                ErrorMessage = result.Error;
            }
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadQuestionsAsync();
    }
}
