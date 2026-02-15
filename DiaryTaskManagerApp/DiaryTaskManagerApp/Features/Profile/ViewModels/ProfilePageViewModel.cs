using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace DiaryTaskManagerApp.Features.Profile.ViewModels;

public sealed class ProfilePageViewModel : INotifyPropertyChanged
{
    private string _userName = "";
    private int _completedCount;
    private string? _avatarPath;

    public ProfilePageViewModel()
    {
        SaveCommand = new Command(Save);
        CloseCommand = new Command(() => RequestClose?.Invoke());
        PickAvatarCommand = new Command(async () => await PickAvatar());
        Load();
    }

    public string UserName
    {
        get => _userName;
        set
        {
            var v = (value ?? "").Trim();
            if (v.Length == 0) v = "User";
            Set(ref _userName, v);
        }
    }

    public int CompletedCount
    {
        get => _completedCount;
        private set => Set(ref _completedCount, value);
    }

    public string? AvatarPath
    {
        get => _avatarPath;
        private set => Set(ref _avatarPath, value);
    }

    public bool HasAvatar => !string.IsNullOrEmpty(AvatarPath);

    public string FlamesText => $"{CompletedCount} 🔥";

    public ICommand SaveCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand PickAvatarCommand { get; }

    public event Action? RequestClose;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Load()
    {
        UserName = Preferences.Get("UserName", "User");
        CompletedCount = Preferences.Get("TotalCompletedCount", 0);
        AvatarPath = Preferences.Get("AvatarPath", (string?)null);
        OnPropertyChanged(nameof(FlamesText));
        OnPropertyChanged(nameof(HasAvatar));
    }

    private void Save()
    {
        Preferences.Set("UserName", UserName);
        RequestClose?.Invoke();
    }

    private async Task PickAvatar()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Pick a photo"
            });

            if (result != null)
            {
                var newPath = Path.Combine(FileSystem.AppDataDirectory, $"avatar_{Guid.NewGuid():N}.jpg");
                
                using (var sourceStream = await result.OpenReadAsync())
                using (var destStream = File.Create(newPath))
                {
                    await sourceStream.CopyToAsync(destStream);
                }

                if (!string.IsNullOrEmpty(AvatarPath) && File.Exists(AvatarPath))
                {
                    try { File.Delete(AvatarPath); } catch { }
                }

                AvatarPath = newPath;
                Preferences.Set("AvatarPath", newPath);
                OnPropertyChanged(nameof(HasAvatar));
            }
        }
        catch (Exception)
        {
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}


