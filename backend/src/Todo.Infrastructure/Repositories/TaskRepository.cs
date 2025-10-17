using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions.Persistence;
using Todo.Application.Features.Tasks.Interfaces;
using Todo.Application.Features.Tasks.Contracts;
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

    public async Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskListFilter filter, CancellationToken ct = default)
    {
        IQueryable<TaskItem> q = DbSet.AsNoTracking();

        q = filter.Status switch
        {
            TaskStatusFilter.Open => q.Where(t => !t.IsDone),
            TaskStatusFilter.Done => q.Where(t => t.IsDone),
            _ => q
        };

        q = (filter.Sort, filter.Direction) switch
        {
            (TaskSortField.CreatedAt, SortDirection.Asc) => q.OrderBy(t => t.CreatedAt),
            (TaskSortField.CreatedAt, SortDirection.Desc) => q.OrderByDescending(t => t.CreatedAt),
            (TaskSortField.OrderIndex, SortDirection.Desc) => q.OrderByDescending(t => t.OrderIndex).ThenBy(t => t.CreatedAt),
            _ => q.OrderBy(t => t.OrderIndex).ThenBy(t => t.CreatedAt)
        };

        return await q.ToListAsync(ct);
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

    public async Task ReorderAsync(IEnumerable<ReorderItem> items, CancellationToken ct = default)
    {
        var map = items.ToDictionary(i => i.Id, i => i.OrderIndex);
        var ids = map.Keys.ToArray();
        var tasks = await DbSet.Where(t => ids.Contains(t.Id)).ToListAsync(ct);
        foreach (var t in tasks)
        {
            t.OrderIndex = map[t.Id];
        }
        await Task.CompletedTask;
    }

    public async Task<HashSet<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idArray = ids.Distinct().ToArray();
        var existing = await DbSet
            .Where(t => idArray.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(ct);
        return existing.ToHashSet();
    }
}
