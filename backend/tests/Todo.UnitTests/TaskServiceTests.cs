using FluentAssertions;
using FluentValidation;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Application.Features.Tasks.Interfaces;
using Todo.Application.Features.Tasks.Services;
using Todo.Application.Features.Tasks.Validators;
using Todo.Domain.Entities;
using Xunit;

namespace Todo.UnitTests;

public class TaskServiceTests
{
    private sealed class FakeTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _items = new();
        private int _nextId = 1;

        public Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default)
        {
            item.Id = _nextId++;
            _items.Add(item);
            return Task.FromResult(item);
        }

        public Task DeleteAsync(TaskItem item, CancellationToken ct = default)
        {
            _items.RemoveAll(x => x.Id == item.Id);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TaskItem>> GetAllForListAsync(CancellationToken ct = default)
            => Task.FromResult((IReadOnlyList<TaskItem>)_items
                .OrderBy(t => t.IsDone).ThenBy(t => t.OrderIndex).ThenBy(t => t.CreatedAt).ToList());

        public Task<int> GetNextOrderIndexAsync(CancellationToken ct = default)
            => Task.FromResult(_items.Count == 0 ? 1 : _items.Max(i => i.OrderIndex) + 1);

        public Task<TaskItem?> FindByIdAsync(int id)
            => Task.FromResult(_items.FirstOrDefault(i => i.Id == id));

        public IQueryable<TaskItem> GetAll() => _items.AsQueryable();
        public IQueryable<TaskItem> Find(System.Linq.Expressions.Expression<Func<TaskItem, bool>> predicate) => _items.AsQueryable().Where(predicate);
        public TaskItem? FindById(int id) => _items.FirstOrDefault(i => i.Id == id);
        public TaskItem Save(TaskItem entity) { _items.Add(entity); return entity; }
        public Task<TaskItem> SaveAsync(TaskItem entity) => Task.FromResult(Save(entity));
        public IEnumerable<TaskItem> Save(IEnumerable<TaskItem> entities) { _items.AddRange(entities); return entities; }
        public TaskItem SaveOrUpdate(TaskItem entity) => entity;
        public IEnumerable<TaskItem> SaveOrUpdate(IEnumerable<TaskItem> entities) => entities;
        public Task UpdateAsync(TaskItem item, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IEnumerable<TaskItem>> Update(IEnumerable<TaskItem> entities) => Task.FromResult(entities);
        public TaskItem Update(TaskItem entity) => entity;
        public void Delete(TaskItem entity) => _items.Remove(entity);
        public void Delete(IEnumerable<TaskItem> entities) { foreach (var e in entities) _items.Remove(e); }
        public void DeleteById(int id) => _items.RemoveAll(i => i.Id == id);
    }

    [Fact]
    public async Task CreateAsync_throws_when_invalid_title()
    {
        var repo = new FakeTaskRepository();
        var validator = new CreateTaskRequestValidator();
        var svc = new TaskService(repo, validator);

        Func<Task> act = async () => await svc.CreateAsync(new CreateTaskRequest { Title = "   " });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_trims_title_and_sets_order()
    {
        var repo = new FakeTaskRepository();
        var validator = new CreateTaskRequestValidator();
        var svc = new TaskService(repo, validator);

        var created = await svc.CreateAsync(new CreateTaskRequest { Title = "  Task A  " });

        created.Id.Should().BeGreaterThan(0);
        created.Title.Should().Be("Task A");
        created.OrderIndex.Should().Be(1);
        created.IsDone.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleComplete_toggles_state()
    {
        var repo = new FakeTaskRepository();
        var validator = new CreateTaskRequestValidator();
        var svc = new TaskService(repo, validator);

        var created = await svc.CreateAsync(new CreateTaskRequest { Title = "Task" });
        created.IsDone.Should().BeFalse();

        var toggled = await svc.ToggleCompleteAsync(created.Id);
        toggled!.IsDone.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_returns_true_when_removed()
    {
        var repo = new FakeTaskRepository();
        var validator = new CreateTaskRequestValidator();
        var svc = new TaskService(repo, validator);

        var created = await svc.CreateAsync(new CreateTaskRequest { Title = "Task" });

        var removed = await svc.DeleteAsync(created.Id);
        removed.Should().BeTrue();

        var notFound = await svc.DeleteAsync(999);
        notFound.Should().BeFalse();
    }
}

