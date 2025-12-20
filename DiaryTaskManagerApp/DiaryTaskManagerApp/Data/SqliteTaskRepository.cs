using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public sealed class SqliteTaskRepository : ITaskRepository
{
    private readonly TaskDatabase _db;

    public SqliteTaskRepository(TaskDatabase db)
    {
        _db = db;
    }

    public IReadOnlyList<TaskItem> GetAll()
    {
        return _db.GetAll().Select(e => e.ToModel()).ToList();
    }

    public void Add(TaskItem task)
    {
        _db.Save(TaskItemEntity.FromModel(task));
    }

    public void Delete(string id)
    {
        _db.Delete(id);
    }
}

