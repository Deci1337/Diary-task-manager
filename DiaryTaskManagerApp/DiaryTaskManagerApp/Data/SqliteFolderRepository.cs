using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public sealed class SqliteFolderRepository : IFolderRepository
{
    private readonly TaskDatabase _db;

    public SqliteFolderRepository(TaskDatabase db) => _db = db;

    public IReadOnlyList<Folder> GetAll() =>
        _db.GetAllFolders().Select(e => e.ToModel()).ToList();

    public void Add(Folder folder) =>
        _db.SaveFolder(FolderEntity.FromModel(folder));

    public void Delete(string id) =>
        _db.DeleteFolder(id);
}
