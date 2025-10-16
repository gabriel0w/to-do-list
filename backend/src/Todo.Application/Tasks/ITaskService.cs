using Todo.Domain.Entities;

namespace Todo.Application.Tasks;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskItem> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskItem?> ToggleCompleteAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

