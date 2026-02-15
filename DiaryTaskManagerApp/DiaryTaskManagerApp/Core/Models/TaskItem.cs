using System.ComponentModel;
using System.Runtime.CompilerServices;
using DiaryTaskManagerApp.Infrastructure;

namespace DiaryTaskManagerApp.Core.Models;

public sealed class TaskItem : INotifyPropertyChanged
{
    private bool _isCompleted;
    private DateTimeOffset? _completedAt;

    public required string Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required TaskImportance Importance { get; init; }
    public string? FolderId { get; init; }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTimeOffset? CompletedAt
    {
        get => _completedAt;
        set
        {
            if (_completedAt != value)
            {
                _completedAt = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CompletedAtText));
            }
        }
    }

    public string Emoji => Importance switch
    {
        TaskImportance.Day => "☀️",
        TaskImportance.Week => "☁️",
        _ => "🌑"
    };

    private static TimeZoneInfo MoscowTz => TimeZones.GetMoscow();

    public string CreatedAtText => TimeZoneInfo.ConvertTime(CreatedAt, MoscowTz).ToString("dd.MM.yyyy HH:mm");
    public string? CompletedAtText => CompletedAt is null ? null : TimeZoneInfo.ConvertTime(CompletedAt.Value, MoscowTz).ToString("dd.MM.yyyy HH:mm");

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


