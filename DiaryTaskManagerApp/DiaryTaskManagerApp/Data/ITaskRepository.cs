using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public interface ITaskRepository
{
    IReadOnlyList<TaskItem> GetAll();
    IReadOnlyList<TaskItem> GetByFolderId(string? folderId);
    void Add(TaskItem item);
    void Delete(string id);
}



