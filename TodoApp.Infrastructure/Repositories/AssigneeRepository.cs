using TodoApp.Core.Abstractions;
using TodoApp.Core.Domain;
using TodoApp.Infrastructure.Storage;

namespace TodoApp.Infrastructure.Repositories;

public sealed class AssigneeRepository : IAssigneeRepository
{
    private const string Store = "assignees";
    private readonly IndexedDbInterop _idb;

    public AssigneeRepository(IndexedDbInterop idb) => _idb = idb;

    public Task<Assignee?> GetAsync(Guid id)
        => _idb.GetAsync<Assignee>(Store, id).AsTask();

    public async Task<IReadOnlyList<Assignee>> ListAsync()
        => await _idb.ListAsync<Assignee>(Store);

    public async Task AddAsync(Assignee entity)
    {
        entity.CreatedAtUtc = DateTime.UtcNow;
        entity.UpdatedAtUtc = entity.CreatedAtUtc;
        await _idb.AddAsync(Store, entity);
    }

    public async Task UpdateAsync(Assignee entity)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _idb.AddAsync(Store, entity); // upsert
    }

    public Task DeleteAsync(Guid id)
        => _idb.DeleteAsync(Store, id).AsTask();
}
