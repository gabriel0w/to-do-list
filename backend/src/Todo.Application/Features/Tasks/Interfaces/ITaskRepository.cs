using Todo.Application.Abstractions.Persistence;
using Todo.Domain.Entities;
using Todo.Application.Features.Tasks.Contracts;

namespace Todo.Application.Features.Tasks.Interfaces;

public interface ITaskRepository : IRepository<TaskItem, int>
{
    Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskListFilter filter, CancellationToken ct = default);
    Task<int> GetNextOrderIndexAsync(CancellationToken ct = default);
    Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default);
    Task UpdateAsync(TaskItem item, CancellationToken ct = default);
    Task DeleteAsync(TaskItem item, CancellationToken ct = default);
    Task ReorderAsync(IEnumerable<ReorderItem> items, CancellationToken ct = default);
    Task<HashSet<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
}
