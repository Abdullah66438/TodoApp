using System.Linq; // for OrderBy/ThenBy
using TodoApp.Core.Abstractions;
using TodoApp.Core.Domain;
using TodoApp.Infrastructure.Storage;

namespace TodoApp.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private const string Store = "tasks";
    private readonly IndexedDbInterop _idb;

    public TaskRepository(IndexedDbInterop idb) => _idb = idb;

    // ITaskRepository.GetAsync(Guid)
    public Task<ProjectTask?> GetAsync(Guid id)
        => _idb.GetAsync<ProjectTask>(Store, id).AsTask();

    // ITaskRepository.ListAsync()
    public async Task<IReadOnlyList<ProjectTask>> ListAsync()
    {
        var all = await _idb.ListAsync<ProjectTask>(Store); // ProjectTask[]
        return all
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAtUtc)
            .ToList(); // List<ProjectTask> implements IReadOnlyList<ProjectTask>
    }

    // ITaskRepository.ListByAssigneeAsync(Guid)
    public async Task<IReadOnlyList<ProjectTask>> ListByAssigneeAsync(Guid assigneeId)
    {
        var arr = await _idb.ListTasksByAssigneeAsync<ProjectTask>(assigneeId); // ProjectTask[]
        return arr
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToList();
    }

    // ITaskRepository.AddAsync(ProjectTask)
    public async Task AddAsync(ProjectTask entity)
    {
        var now = DateTime.UtcNow;
        entity.CreatedAtUtc = now;
        entity.UpdatedAtUtc = now;

        if (entity.DueDate.HasValue)
            entity.DueDate = DateTime.SpecifyKind(entity.DueDate.Value, DateTimeKind.Utc);

        // Ensure Id exists — prevents "key path did not yield a value"
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        await _idb.AddAsync(Store, entity);
    }

    // ITaskRepository.UpdateAsync(ProjectTask)
    public async Task UpdateAsync(ProjectTask entity)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;

        if (entity.DueDate.HasValue)
            entity.DueDate = DateTime.SpecifyKind(entity.DueDate.Value, DateTimeKind.Utc);

        // Ensure Id exists (should exist when updating, but guard just in case)
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        await _idb.AddAsync(Store, entity); // put = upsert
    }

    // ITaskRepository.DeleteAsync(Guid)
    public Task DeleteAsync(Guid id)
        => _idb.DeleteAsync(Store, id).AsTask();
}
