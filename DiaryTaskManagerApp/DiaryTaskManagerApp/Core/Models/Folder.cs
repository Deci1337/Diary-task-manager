namespace DiaryTaskManagerApp.Core.Models;

public sealed class Folder
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
