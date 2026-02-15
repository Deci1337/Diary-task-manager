using SQLite;
using DiaryTaskManagerApp.Core.Models;
using DiaryTaskManagerApp.Infrastructure;

namespace DiaryTaskManagerApp.Data;

public sealed class TaskDatabase
{
    private readonly SQLiteConnection _connection;
    private static readonly TimeZoneInfo MoscowTz = TimeZones.GetMoscow();

    public TaskDatabase(string dbPath)
    {
        _connection = new SQLiteConnection(dbPath);
        _connection.CreateTable<TaskItemEntity>();
        _connection.CreateTable<FolderEntity>();
        MigrateAddFolderId();
        MigrateLegacyTicksAssumingMoscow();
    }

    private void MigrateAddFolderId()
    {
        try
        {
            _connection.Execute("ALTER TABLE Tasks ADD COLUMN FolderId TEXT");
        }
        catch
        {
        }
    }

    private void MigrateLegacyTicksAssumingMoscow()
    {
        // Old format stored DateTimeOffset.Ticks (local clock time). We now store UtcTicks.
        // If legacy data exists, interpreting it as UTC will shift by +3h in Moscow.
        var sample = _connection.Table<TaskItemEntity>().FirstOrDefault();
        if (sample is null) return;

        var asUtc = new DateTimeOffset(sample.CreatedAtTicks, TimeSpan.Zero);
        if (asUtc > DateTimeOffset.UtcNow.AddHours(1))
        {
            var all = _connection.Table<TaskItemEntity>().ToList();
            foreach (var x in all)
            {
                x.CreatedAtTicks = ToUtcTicksAssumingMoscowLocalTicks(x.CreatedAtTicks);
                x.CompletedAtTicks = x.CompletedAtTicks.HasValue ? ToUtcTicksAssumingMoscowLocalTicks(x.CompletedAtTicks.Value) : null;
                _connection.InsertOrReplace(x);
            }
        }
    }

    private static long ToUtcTicksAssumingMoscowLocalTicks(long localTicks)
    {
        var local = new DateTime(localTicks, DateTimeKind.Unspecified);
        var offset = MoscowTz.GetUtcOffset(local);
        return new DateTimeOffset(local, offset).UtcTicks;
    }

    public List<TaskItemEntity> GetAll()
    {
        return _connection.Table<TaskItemEntity>().ToList();
    }

    public List<TaskItemEntity> GetByFolderId(string? folderId)
    {
        if (string.IsNullOrEmpty(folderId))
            return _connection.Table<TaskItemEntity>().Where(x => x.FolderId == null || x.FolderId == "").ToList();
        return _connection.Table<TaskItemEntity>().Where(x => x.FolderId == folderId).ToList();
    }

    public List<FolderEntity> GetAllFolders()
    {
        return _connection.Table<FolderEntity>().ToList();
    }

    public int SaveFolder(FolderEntity folder)
    {
        return _connection.InsertOrReplace(folder);
    }

    public int DeleteFolder(string id)
    {
        _connection.Table<TaskItemEntity>().Where(x => x.FolderId == id).ToList().ForEach(t => Delete(t.Id));
        return _connection.Table<FolderEntity>().Delete(x => x.Id == id);
    }

    public TaskItemEntity? GetById(string id)
    {
        return _connection.Table<TaskItemEntity>().FirstOrDefault(x => x.Id == id);
    }

    public int Save(TaskItemEntity item)
    {
        return _connection.InsertOrReplace(item);
    }

    public int Delete(string id)
    {
        return _connection.Table<TaskItemEntity>().Delete(x => x.Id == id);
    }
}

[Table("Tasks")]
public sealed class TaskItemEntity
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    public string Title { get; set; } = "";
    
    public long CreatedAtTicks { get; set; }
    
    public int Importance { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public long? CompletedAtTicks { get; set; }

    public string? FolderId { get; set; }

    public static TaskItemEntity FromModel(TaskItem model) => new()
    {
        Id = model.Id,
        Title = model.Title,
        CreatedAtTicks = model.CreatedAt.UtcTicks,
        Importance = (int)model.Importance,
        IsCompleted = model.IsCompleted,
        CompletedAtTicks = model.CompletedAt?.UtcTicks,
        FolderId = model.FolderId
    };

    public TaskItem ToModel() => new()
    {
        Id = Id,
        Title = Title,
        CreatedAt = new DateTimeOffset(CreatedAtTicks, TimeSpan.Zero),
        Importance = (TaskImportance)Importance,
        IsCompleted = IsCompleted,
        CompletedAt = CompletedAtTicks.HasValue ? new DateTimeOffset(CompletedAtTicks.Value, TimeSpan.Zero) : null,
        FolderId = FolderId
    };
}

[Table("Folders")]
public sealed class FolderEntity
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";
    public long CreatedAtTicks { get; set; }

    public static FolderEntity FromModel(Folder model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        CreatedAtTicks = model.CreatedAt.UtcTicks
    };

    public Folder ToModel() => new()
    {
        Id = Id,
        Name = Name,
        CreatedAt = new DateTimeOffset(CreatedAtTicks, TimeSpan.Zero)
    };
}

