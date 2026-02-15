using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiaryTaskManagerApp.Features.Settings.ViewModels;

public sealed class SettingsPageViewModel : INotifyPropertyChanged
{
    public string Version => "0.2.1";
    public string BuildDate { get; }
    public string GitHubUrl => "https://github.com/Deci1337/Diary-task-manager/tree/main/DiaryTaskManagerApp";

    public SettingsPageViewModel()
    {
        try
        {
            var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            BuildDate = !string.IsNullOrEmpty(loc) && System.IO.File.Exists(loc)
                ? new FileInfo(loc).LastWriteTime.ToString("dd.MM.yyyy") : "—";
        }
        catch
        {
            BuildDate = "—";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
