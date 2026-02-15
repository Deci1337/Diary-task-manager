using System.ComponentModel;
using System.Runtime.CompilerServices;
using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Data;

namespace DiaryTaskManagerApp.Features.Folders.ViewModels;

public sealed class FolderItemViewModel : INotifyPropertyChanged
{
    private readonly ITaskRepository _taskRepo;

    public FolderItemViewModel(Folder folder, ITaskRepository taskRepo)
    {
        Folder = folder;
        _taskRepo = taskRepo;
    }

    public Folder Folder { get; }

    public int TaskCount => _taskRepo.GetByFolderId(Folder.Id).Count();

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
