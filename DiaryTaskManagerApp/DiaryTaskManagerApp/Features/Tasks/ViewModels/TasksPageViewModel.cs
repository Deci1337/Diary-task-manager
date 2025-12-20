using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Data;
using Microsoft.Maui.Storage;

namespace DiaryTaskManagerApp.Features.Tasks.ViewModels;

public sealed class TasksPageViewModel : INotifyPropertyChanged
{
    private readonly ITaskRepository _repo;
    private string _newTaskText = "";
    private int _selectedImportanceIndex;
    private int _selectedFilterIndex;
    private int _selectedSortIndex;
    private string _userName = "";
    private int _completedCount;
    private string? _avatarPath;

    public TasksPageViewModel(ITaskRepository repo)
    {
        _repo = repo;
        Tasks = new ObservableCollection<TaskItem>();

        FilterOptions = ["Все", "☀️", "☁️", "🌑", "✓", "✕"];
        SortOptions = ["Новые", "Старые"];

        CycleImportanceCommand = new Command(CycleImportance);
        SelectImportanceCommand = new Command<int>(i => SelectedImportanceIndex = i);
        AddTaskCommand = new Command(AddTask);
        DeleteTaskCommand = new Command<TaskItem>(DeleteTask);
        ToggleCompleteCommand = new Command<TaskItem>(ToggleComplete);

        LoadHeader();
        Refresh();
    }

    public ObservableCollection<TaskItem> Tasks { get; }
    public string[] FilterOptions { get; }
    public string[] SortOptions { get; }

    public string UserName
    {
        get => _userName;
        set
        {
            var v = (value ?? "").Trim();
            if (v.Length == 0) v = "Пользователь";
            if (Set(ref _userName, v))
            {
                Preferences.Set("UserName", v);
            }
        }
    }

    public int CompletedCount
    {
        get => _completedCount;
        private set
        {
            if (Set(ref _completedCount, value))
            {
                OnPropertyChanged(nameof(FlamesText));
                OnPropertyChanged(nameof(HasFlames));
            }
        }
    }

    public bool HasFlames => CompletedCount > 0;
    public string FlamesText => $"{CompletedCount} 🔥";

    public string? AvatarPath
    {
        get => _avatarPath;
        private set
        {
            if (Set(ref _avatarPath, value))
            {
                OnPropertyChanged(nameof(HasAvatar));
            }
        }
    }

    public bool HasAvatar => !string.IsNullOrEmpty(AvatarPath);

    public string NewTaskText
    {
        get => _newTaskText;
        set { if (Set(ref _newTaskText, value)) ((Command)AddTaskCommand).ChangeCanExecute(); }
    }

    public int SelectedImportanceIndex
    {
        get => _selectedImportanceIndex;
        set
        {
            if (Set(ref _selectedImportanceIndex, value))
            {
                OnPropertyChanged(nameof(CurrentImportanceEmoji));
            }
        }
    }

    public string CurrentImportanceEmoji => SelectedImportanceIndex switch
    {
        0 => "☀️",
        1 => "☁️",
        2 => "🌑",
        _ => "☀️"
    };

    public int SelectedFilterIndex
    {
        get => _selectedFilterIndex;
        set { if (Set(ref _selectedFilterIndex, value)) Refresh(); }
    }

    public int SelectedSortIndex
    {
        get => _selectedSortIndex;
        set { if (Set(ref _selectedSortIndex, value)) Refresh(); }
    }

    public ICommand CycleImportanceCommand { get; }
    public ICommand SelectImportanceCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleCompleteCommand { get; }

    public void LoadHeader()
    {
        UserName = Preferences.Get("UserName", "Пользователь");
        CompletedCount = Preferences.Get("TotalCompletedCount", 0);
        AvatarPath = Preferences.Get("AvatarPath", (string?)null);
    }

    private void CycleImportance()
    {
        SelectedImportanceIndex = (SelectedImportanceIndex + 1) % 3;
    }

    private void DeleteTask(TaskItem? task)
    {
        if (task is null) return;
        _repo.Delete(task.Id);
        Refresh();
    }

    private void ToggleComplete(TaskItem? task)
    {
        if (task is null) return;

        var wasCompleted = task.IsCompleted;
        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTimeOffset.Now : null;
        
        if (task.IsCompleted && !wasCompleted)
        {
            CompletedCount++;
            Preferences.Set("TotalCompletedCount", CompletedCount);
        }
        else if (!task.IsCompleted && wasCompleted)
        {
            CompletedCount = Math.Max(0, CompletedCount - 1);
            Preferences.Set("TotalCompletedCount", CompletedCount);
        }
        
        _repo.Add(task);
        Refresh();
    }

    private void AddTask()
    {
        var title = (NewTaskText ?? "").Trim();
        if (title.Length == 0) return;

        _repo.Add(new TaskItem
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title,
            CreatedAt = DateTimeOffset.Now,
            Importance = (TaskImportance)SelectedImportanceIndex
        });

        NewTaskText = "";
        Refresh();
    }

    private void Refresh()
    {
        var existingById = Tasks.ToDictionary(x => x.Id, x => x);

        var all = _repo.GetAll();

        IEnumerable<TaskItem> items = all;

        if (SelectedFilterIndex is 1 or 2 or 3)
        {
            var importance = (TaskImportance)(SelectedFilterIndex - 1);
            items = items.Where(x => x.Importance == importance);
        }
        else if (SelectedFilterIndex == 4)
        {
            items = items.Where(x => x.IsCompleted);
        }
        else if (SelectedFilterIndex == 5)
        {
            items = items.Where(x => !x.IsCompleted);
        }

        items = SelectedSortIndex == 0
            ? items.OrderByDescending(x => x.CreatedAt)
            : items.OrderBy(x => x.CreatedAt);

        var desired = new List<TaskItem>();
        foreach (var fromRepo in items)
        {
            if (existingById.TryGetValue(fromRepo.Id, out var existing))
            {
                // Keep the same instance so bindings/triggers keep working.
                existing.IsCompleted = fromRepo.IsCompleted;
                existing.CompletedAt = fromRepo.CompletedAt;
                desired.Add(existing);
            }
            else
            {
                desired.Add(fromRepo);
            }
        }

        Tasks.Clear();
        foreach (var t in desired) Tasks.Add(t);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
}


