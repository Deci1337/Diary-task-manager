using DiaryTaskManagerApp.Features.Settings.ViewModels;

namespace DiaryTaskManagerApp.Features.Settings.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnMenuTapped(object? sender, TappedEventArgs e)
    {
        if (Shell.Current is Shell shell)
            shell.FlyoutIsPresented = true;
    }

    private async void OnGitHubTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is SettingsPageViewModel vm)
            await Launcher.OpenAsync(vm.GitHubUrl);
    }
}
