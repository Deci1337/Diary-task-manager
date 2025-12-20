using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public interface ITaskRepository
{
    IReadOnlyList<TaskItem> GetAll();
    void Add(TaskItem item);
    void Delete(string id);
}



