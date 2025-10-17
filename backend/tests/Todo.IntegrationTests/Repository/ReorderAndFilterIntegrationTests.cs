using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Persistence.Db;
using Todo.Infrastructure.Repositories;
using Todo.IntegrationTests.Fixtures;
using Xunit;

namespace Todo.IntegrationTests.Repository;

public class ReorderAndFilterIntegrationTests : IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresContainerFixture _fx;
    public ReorderAndFilterIntegrationTests(PostgresContainerFixture fx) => _fx = fx;

    private (TodoDbContext db, UnitOfWork<TodoDbContext> uow, TaskRepository repo) CreateScope()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;
        var db = new TodoDbContext(options);
        var uow = new UnitOfWork<TodoDbContext>(db);
        var repo = new TaskRepository(db, uow);
        return (db, uow, repo);
    }

    [Fact]
    public async Task Reorder_is_idempotent_and_stable()
    {
        var (db, uow, repo) = CreateScope();
        try
        {
            // seed
            await repo.AddAsync(new TaskItem { Title = "A", OrderIndex = 1 });
            await repo.AddAsync(new TaskItem { Title = "B", OrderIndex = 2 });
            await repo.AddAsync(new TaskItem { Title = "C", OrderIndex = 3 });
            await uow.CommitAsync();

            var items = await repo.GetAllForListAsync();
            var a = items.Single(t => t.Title == "A");
            var b = items.Single(t => t.Title == "B");
            var c = items.Single(t => t.Title == "C");

            var reorder = new[]
            {
                new ReorderItem { Id = a.Id, OrderIndex = 3 },
                new ReorderItem { Id = b.Id, OrderIndex = 1 },
                new ReorderItem { Id = c.Id, OrderIndex = 2 },
            };

            await repo.ReorderAsync(reorder);
            await uow.CommitAsync();

            var after = await repo.GetAllForListAsync();
            after.Select(t => (t.Title, t.OrderIndex)).Should().Contain(new[]
            {
                ("A", 3), ("B", 1), ("C", 2)
            });

            // repeat same reorder
            await repo.ReorderAsync(reorder);
            await uow.CommitAsync();

            var after2 = await repo.GetAllForListAsync();
            after2.Select(t => (t.Title, t.OrderIndex)).Should().BeEquivalentTo(after.Select(t => (t.Title, t.OrderIndex)));
        }
        finally
        {
            db.Dispose();
        }
    }

    [Fact]
    public async Task Filters_work_for_status_and_sort()
    {
        var (db, uow, repo) = CreateScope();
        try
        {
            db.Tasks.RemoveRange(db.Tasks);
            await db.SaveChangesAsync();

            await repo.AddAsync(new TaskItem { Title = "X", OrderIndex = 2, IsDone = false, CreatedAt = DateTime.UtcNow.AddMinutes(-2) });
            await repo.AddAsync(new TaskItem { Title = "Y", OrderIndex = 1, IsDone = true, CreatedAt = DateTime.UtcNow.AddMinutes(-1) });
            await repo.AddAsync(new TaskItem { Title = "Z", OrderIndex = 3, IsDone = false, CreatedAt = DateTime.UtcNow });
            await uow.CommitAsync();

            var openAsc = await repo.GetFilteredAsync(new TaskListFilter { Status = TaskStatusFilter.Open, Sort = TaskSortField.OrderIndex, Direction = SortDirection.Asc });
            openAsc.Select(t => t.Title).Should().ContainInOrder("X", "Z");

            var done = await repo.GetFilteredAsync(new TaskListFilter { Status = TaskStatusFilter.Done });
            done.Should().ContainSingle(t => t.Title == "Y");

            var createdDesc = await repo.GetFilteredAsync(new TaskListFilter { Status = TaskStatusFilter.All, Sort = TaskSortField.CreatedAt, Direction = SortDirection.Desc });
            createdDesc.First().Title.Should().Be("Z");
        }
        finally
        {
            db.Dispose();
        }
    }
}

