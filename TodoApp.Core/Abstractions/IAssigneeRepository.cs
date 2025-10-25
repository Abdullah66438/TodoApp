using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Core.Domain;

namespace TodoApp.Core.Abstractions
{
    public interface IAssigneeRepository
    {
        Task<Assignee?> GetAsync(Guid id);
        Task<IReadOnlyList<Assignee>> ListAsync();
        Task AddAsync(Assignee entity);
        Task UpdateAsync(Assignee entity);
        Task DeleteAsync(Guid id);
    }
}
