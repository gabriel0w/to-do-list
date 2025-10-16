using Todo.Domain.Entities;

namespace Todo.Application.Tasks;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> GetNextOrderIndexAsync(CancellationToken ct = default);
    Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default);
    Task UpdateAsync(TaskItem item, CancellationToken ct = default);
    Task DeleteAsync(TaskItem item, CancellationToken ct = default);
}

