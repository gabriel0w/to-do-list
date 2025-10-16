using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions.Persistence;
using Todo.Application.Features.Tasks.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence.Db;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class TaskRepository : AbstractRepository<TodoDbContext, TaskItem, int>, ITaskRepository
{
    public TaskRepository(TodoDbContext db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
    { }

    public async Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(t => t.IsDone)
            .ThenBy(t => t.OrderIndex)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetNextOrderIndexAsync(CancellationToken ct = default)
    {
        var max = await DbSet.MaxAsync(t => (int?)t.OrderIndex, ct) ?? 0;
        return max + 1;
    }

    public async Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default)
    {
        await DbSet.AddAsync(item, ct);
        return item;
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        DbSet.Update(item);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TaskItem item, CancellationToken ct = default)
    {
        DbSet.Remove(item);
        await Task.CompletedTask;
    }
}
