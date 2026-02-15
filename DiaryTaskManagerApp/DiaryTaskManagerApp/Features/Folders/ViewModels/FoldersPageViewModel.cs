using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Data;

namespace DiaryTaskManagerApp.Features.Folders.ViewModels;

public sealed class FoldersPageViewModel : INotifyPropertyChanged
{
    private readonly ITaskRepository _taskRepo;
    private readonly IFolderRepository _folderRepo;
    private string _newFolderName = "";
    private int _selectedSortIndex;

    public FoldersPageViewModel(ITaskRepository taskRepo, IFolderRepository folderRepo)
    {
        _taskRepo = taskRepo;
        _folderRepo = folderRepo;
        Folders = new ObservableCollection<FolderItemViewModel>();
        SortOptions = ["By important", "By task count", "By date"];
        CreateFolderCommand = new Command(CreateFolder, () => !string.IsNullOrWhiteSpace(NewFolderName));
        Refresh();
    }

    public ObservableCollection<FolderItemViewModel> Folders { get; }
    public string[] SortOptions { get; }

    public string NewFolderName
    {
        get => _newFolderName;
        set { if (Set(ref _newFolderName, value)) ((Command)CreateFolderCommand).ChangeCanExecute(); }
    }


    public int SelectedSortIndex
    {
        get => _selectedSortIndex;
        set { if (Set(ref _selectedSortIndex, value)) Refresh(); }
    }

    public ICommand CreateFolderCommand { get; }

    public void Refresh()
    {
        var folders = _folderRepo.GetAll();
        var sorted = SelectedSortIndex switch
        {
            0 => folders.OrderByDescending(f => _taskRepo.GetByFolderId(f.Id).Count(t => t.Importance == TaskImportance.Day && !t.IsCompleted)),
            1 => folders.OrderByDescending(f => _taskRepo.GetByFolderId(f.Id).Count),
            _ => folders.OrderByDescending(f => f.CreatedAt)
        };
        Folders.Clear();
        foreach (var f in sorted)
        {
            var vm = new FolderItemViewModel(f, _taskRepo);
            Folders.Add(vm);
        }
    }

    private void CreateFolder()
    {
        var name = (NewFolderName ?? "").Trim();
        if (name.Length == 0) return;
        _folderRepo.Add(new Folder
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = name,
            CreatedAt = DateTimeOffset.Now
        });
        NewFolderName = "";
        Refresh();
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
