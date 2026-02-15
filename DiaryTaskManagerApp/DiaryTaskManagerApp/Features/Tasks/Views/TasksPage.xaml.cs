using DiaryTaskManagerApp.Features.Tasks.ViewModels;
using DiaryTaskManagerApp.Features.Profile.Views;
using DiaryTaskManagerApp.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DiaryTaskManagerApp.Features.Tasks.Views;

public partial class TasksPage : ContentPage
{
    public TasksPageViewModel ViewModel { get; }
    private readonly IServiceProvider _services;
    private readonly Dictionary<Border, CancellationTokenSource> _longPressTokens = new();
    private const int LongPressDurationMs = 700;

    public TasksPage(TasksPageViewModel vm, IServiceProvider services)
    {
        InitializeComponent();
        ViewModel = vm;
        _services = services;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadHeader();
    }

    private void OnMenuTapped(object? sender, TappedEventArgs e)
    {
        if (Shell.Current is Shell shell)
            shell.FlyoutIsPresented = true;
    }

    private async void OnProfileTapped(object? sender, TappedEventArgs e)
    {
        var page = _services.GetRequiredService<ProfilePage>();
        await Navigation.PushModalAsync(page);
    }

    private void OnTaskTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is TaskItem task)
        {
            ViewModel.ToggleCompleteCommand.Execute(task);
        }
    }

    private void OnTaskPressed(object? sender, PointerEventArgs e)
    {
        if (sender is not Border border || border.BindingContext is not TaskItem task) return;

        if (_longPressTokens.TryGetValue(border, out var existingCts))
        {
            existingCts.Cancel();
            existingCts.Dispose();
        }

        var cts = new CancellationTokenSource();
        _longPressTokens[border] = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(LongPressDurationMs, cts.Token);
                if (!cts.Token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ViewModel.DeleteTaskCommand.Execute(task);
                    });
                }
            }
            catch (TaskCanceledException)
            {
            }
        });
    }

    private void OnTaskReleased(object? sender, PointerEventArgs e)
    {
        if (sender is Border border && _longPressTokens.TryGetValue(border, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _longPressTokens.Remove(border);
        }
    }

    private void OnTaskExited(object? sender, PointerEventArgs e)
    {
        if (sender is Border border && _longPressTokens.TryGetValue(border, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _longPressTokens.Remove(border);
        }
    }

    private void OnSwipeDelete(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TaskItem task)
        {
            ViewModel.DeleteTaskCommand.Execute(task);
        }
    }
}



