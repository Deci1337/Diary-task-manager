using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Features.Folders.ViewModels;

namespace DiaryTaskManagerApp.Features.Folders.Views;

public partial class FolderDetailsPage : ContentPage
{
    public FolderDetailsPage(FolderDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnTaskTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Element el || el.BindingContext is not TaskItem task) return;
        if (BindingContext is FolderDetailsViewModel vm)
            vm.ToggleCompleteCommand.Execute(task);
    }

    private void OnSwipeDelete(object? sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem || swipeItem.BindingContext is not TaskItem task) return;
        if (BindingContext is FolderDetailsViewModel vm)
            vm.DeleteTaskCommand.Execute(task);
    }
}
