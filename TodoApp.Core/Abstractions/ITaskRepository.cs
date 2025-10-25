using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Core.Domain;

namespace TodoApp.Core.Abstractions
{
    public interface ITaskRepository
    {
        Task<ProjectTask?> GetAsync(Guid id);
        Task<IReadOnlyList<ProjectTask>> ListAsync();
        Task<IReadOnlyList<ProjectTask>> ListByAssigneeAsync(Guid assigneeId);
        Task AddAsync(ProjectTask entity);
        Task UpdateAsync(ProjectTask entity);
        Task DeleteAsync(Guid id);
    }
}
