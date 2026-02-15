using DiaryTaskManagerApp.Features.Folders.ViewModels;

namespace DiaryTaskManagerApp.Features.Folders.Views;

public partial class FoldersPage : ContentPage
{
    public FoldersPage(FoldersPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnMenuTapped(object? sender, TappedEventArgs e)
    {
        if (Shell.Current is Shell shell)
            shell.FlyoutIsPresented = true;
    }

    private async void OnFolderTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Element el || el.BindingContext is not FolderItemViewModel fvm) return;
        await Shell.Current.GoToAsync($"FolderDetails?folderId={fvm.Folder.Id}");
    }
}
