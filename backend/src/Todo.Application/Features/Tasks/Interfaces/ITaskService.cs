using Todo.Domain.Entities;
using Todo.Application.Features.Tasks.Contracts;

namespace Todo.Application.Features.Tasks.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(TaskListFilter filter, CancellationToken ct = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskItem> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskItem?> ToggleCompleteAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task ReorderAsync(ReorderTasksRequest request, CancellationToken ct = default);
}
