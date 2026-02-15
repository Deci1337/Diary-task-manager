using DiaryTaskManagerApp.Core.Models;

namespace DiaryTaskManagerApp.Data;

public interface IFolderRepository
{
    IReadOnlyList<Folder> GetAll();
    void Add(Folder folder);
    void Delete(string id);
}
