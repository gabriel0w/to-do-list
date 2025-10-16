using FluentValidation;
using Todo.Domain.Entities;

namespace Todo.Application.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IValidator<CreateTaskRequest> _createValidator;

    public TaskService(ITaskRepository repo, IValidator<CreateTaskRequest> createValidator)
    {
        _repo = repo;
        _createValidator = createValidator;
    }

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllForListAsync(ct);

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.FindByIdAsync(id);

    public async Task<TaskItem> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            var errs = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errs);
        }

        var nextOrder = await _repo.GetNextOrderIndexAsync(ct);
        var item = new TaskItem
        {
            Title = request.Title.Trim(),
            IsDone = false,
            OrderIndex = nextOrder,
            CreatedAt = DateTime.UtcNow
        };

        return await _repo.AddAsync(item, ct);
    }

    public async Task<TaskItem?> ToggleCompleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.FindByIdAsync(id);
        if (item is null) return null;
        item.IsDone = !item.IsDone;
        await _repo.UpdateAsync(item, ct);
        return item;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.FindByIdAsync(id);
        if (item is null) return false;
        await _repo.DeleteAsync(item, ct);
        return true;
    }
}
