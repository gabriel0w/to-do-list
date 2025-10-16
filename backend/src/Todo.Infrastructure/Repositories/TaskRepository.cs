using Microsoft.EntityFrameworkCore;
using Todo.Application.Tasks;
using Todo.Domain.Entities;
using Todo.Infrastructure.Data;

namespace Todo.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoDbContext _db;

    public TaskRepository(TodoDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default)
    {
        return await _db.Tasks
            .AsNoTracking()
            .OrderBy(t => t.IsDone)
            .ThenBy(t => t.OrderIndex)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct)!;

    public async Task<int> GetNextOrderIndexAsync(CancellationToken ct = default)
    {
        var max = await _db.Tasks.MaxAsync(t => (int?)t.OrderIndex, ct) ?? 0;
        return max + 1;
    }

    public async Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default)
    {
        _db.Tasks.Add(item);
        await _db.SaveChangesAsync(ct);
        return item;
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        _db.Tasks.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TaskItem item, CancellationToken ct = default)
    {
        _db.Tasks.Remove(item);
        await _db.SaveChangesAsync(ct);
    }
}

