using FluentValidation;
using Microsoft.Extensions.Logging;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Application.Features.Tasks.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Features.Tasks.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly ILogger<TaskService> _logger;
    private readonly IValidator<CreateTaskRequest> _createValidator;

    public TaskService(ITaskRepository repo, IValidator<CreateTaskRequest> createValidator, ILogger<TaskService> logger)
    {
        _repo = repo;
        _createValidator = createValidator;
        _logger = logger;
    }

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllForListAsync(ct);

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(TaskListFilter filter, CancellationToken ct = default)
        => _repo.GetFilteredAsync(filter, ct);

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.FindByIdAsync(id);

    public async Task<TaskItem> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var nextOrder = await _repo.GetNextOrderIndexAsync(ct);
        var item = new TaskItem
        {
            Title = request.Title.Trim(),
            IsDone = false,
            OrderIndex = nextOrder,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.AddAsync(item, ct);
        _logger.LogInformation("Task created: {TaskId} - '{Title}' with OrderIndex {OrderIndex}", created.Id, created.Title, created.OrderIndex);
        return created;
    }

    public async Task<TaskItem?> ToggleCompleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.FindByIdAsync(id);
        if (item is null) return null;
        item.IsDone = !item.IsDone;
        await _repo.UpdateAsync(item, ct);
        _logger.LogInformation("Task toggled completion: {TaskId} now IsDone={IsDone}", item.Id, item.IsDone);
        return item;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.FindByIdAsync(id);
        if (item is null) return false;
        await _repo.DeleteAsync(item, ct);
        _logger.LogInformation("Task deleted: {TaskId}", id);
        return true;
    }

    public async Task ReorderAsync(ReorderTasksRequest request, CancellationToken ct = default)
    {
        var validator = new Validators.ReorderTasksRequestValidator();
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var ids = request.Items.Select(i => i.Id).ToArray();
        var existing = await _repo.GetExistingIdsAsync(ids, ct);
        var missing = ids.Where(id => !existing.Contains(id)).ToArray();
        if (missing.Length > 0)
        {
            var failures = missing.Select(id => new FluentValidation.Results.ValidationFailure("Items", $"Task id {id} not found"));
            throw new ValidationException(failures);
        }

        await _repo.ReorderAsync(request.Items, ct);
        _logger.LogInformation("Tasks reordered: {Count} items", request.Items.Count);
    }
}
