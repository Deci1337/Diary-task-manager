using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _items = [];

    public IReadOnlyList<TaskItem> GetAll() => _items;

    public IReadOnlyList<TaskItem> GetByFolderId(string? folderId) =>
        _items.Where(x => (folderId == null || folderId == "") ? string.IsNullOrEmpty(x.FolderId) : x.FolderId == folderId).ToList();

    public void Add(TaskItem item)
    {
        // Upsert by Id (so toggling complete can be persisted without a separate Update method).
        var index = _items.FindIndex(x => x.Id == item.Id);
        if (index >= 0) _items[index] = item;
        else _items.Add(item);
    }

    public void Delete(string id) => _items.RemoveAll(x => x.Id == id);
}



