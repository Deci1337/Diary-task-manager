using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Data;

namespace DiaryTaskManagerApp.Features.Folders.ViewModels;

public sealed class FolderDetailsViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly ITaskRepository _taskRepo;
    private readonly IFolderRepository _folderRepo;
    private string _folderId = "";
    private string _folderName = "";
    private string _newTaskText = "";
    private int _selectedImportanceIndex;

    public FolderDetailsViewModel(ITaskRepository taskRepo, IFolderRepository folderRepo)
    {
        _taskRepo = taskRepo;
        _folderRepo = folderRepo;
        Tasks = new ObservableCollection<TaskItem>();
        AddTaskCommand = new Command(AddTask, () => !string.IsNullOrWhiteSpace(NewTaskText));
        DeleteTaskCommand = new Command<TaskItem>(DeleteTask);
        ToggleCompleteCommand = new Command<TaskItem>(ToggleComplete);
        DeleteFolderCommand = new Command(DeleteFolder);
        CycleImportanceCommand = new Command(() => SelectedImportanceIndex = (SelectedImportanceIndex + 1) % 3);
    }

    public ObservableCollection<TaskItem> Tasks { get; }

    public string FolderName
    {
        get => _folderName;
        private set => Set(ref _folderName, value);
    }

    public string NewTaskText
    {
        get => _newTaskText;
        set { if (Set(ref _newTaskText, value)) ((Command)AddTaskCommand).ChangeCanExecute(); }
    }

    public int SelectedImportanceIndex
    {
        get => _selectedImportanceIndex;
        set { if (Set(ref _selectedImportanceIndex, value)) OnPropertyChanged(nameof(CurrentImportanceEmoji)); }
    }

    public string CurrentImportanceEmoji => SelectedImportanceIndex switch { 0 => "☀️", 1 => "☁️", _ => "🌑" };

    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleCompleteCommand { get; }
    public ICommand DeleteFolderCommand { get; }
    public ICommand CycleImportanceCommand { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("folderId", out var id) && id is string folderId)
        {
            _folderId = folderId;
            var folder = _folderRepo.GetAll().FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                FolderName = folder.Name;
                LoadTasks();
            }
        }
    }

    private void LoadTasks()
    {
        Tasks.Clear();
        foreach (var t in _taskRepo.GetByFolderId(_folderId))
            Tasks.Add(t);
    }

    private void AddTask()
    {
        var title = (NewTaskText ?? "").Trim();
        if (title.Length == 0) return;
        _taskRepo.Add(new TaskItem
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title,
            CreatedAt = DateTimeOffset.Now,
            Importance = (TaskImportance)SelectedImportanceIndex,
            FolderId = _folderId
        });
        NewTaskText = "";
        LoadTasks();
    }

    private void DeleteTask(TaskItem? task)
    {
        if (task is null) return;
        _taskRepo.Delete(task.Id);
        LoadTasks();
    }

    private void ToggleComplete(TaskItem? task)
    {
        if (task is null) return;
        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTimeOffset.Now : null;
        _taskRepo.Add(task);
        LoadTasks();
    }

    private async void DeleteFolder()
    {
        _folderRepo.Delete(_folderId);
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
}
