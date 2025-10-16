using Todo.Application.Abstractions.Persistence;
using Todo.Domain.Entities;

namespace Todo.Application.Features.Tasks.Interfaces;

public interface ITaskRepository : IRepository<TaskItem, int>
{
    Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default);
    Task<int> GetNextOrderIndexAsync(CancellationToken ct = default);
    Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default);
    Task UpdateAsync(TaskItem item, CancellationToken ct = default);
    Task DeleteAsync(TaskItem item, CancellationToken ct = default);
}
