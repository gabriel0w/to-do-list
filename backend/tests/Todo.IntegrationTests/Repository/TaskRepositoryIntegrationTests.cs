using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Persistence.Db;
using Todo.Infrastructure.Repositories;
using Todo.IntegrationTests.Fixtures;
using Xunit;

namespace Todo.IntegrationTests.Repository;

public class TaskRepositoryIntegrationTests : IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresContainerFixture _fx;

    public TaskRepositoryIntegrationTests(PostgresContainerFixture fx)
    {
        _fx = fx;
    }

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
    public async Task Add_and_query_tasks_persist_in_database()
    {
        var (db, uow, repo) = CreateScope();
        try
        {
            var t = new TaskItem { Title = "from-it", OrderIndex = 1 };
            await repo.AddAsync(t);
            await uow.CommitAsync();

            var all = await repo.GetAllForListAsync();
            all.Should().ContainSingle(x => x.Title == "from-it");
        }
        finally
        {
            db.Dispose();
        }
    }

    [Fact]
    public async Task Toggle_and_delete_affect_persistence()
    {
        var (db, uow, repo) = CreateScope();
        try
        {
            var t = new TaskItem { Title = "to-toggle", OrderIndex = 1 };
            await repo.AddAsync(t);
            await uow.CommitAsync();

            var loaded = await repo.FindByIdAsync(t.Id);
            loaded!.IsDone.Should().BeFalse();
            loaded.IsDone = true;
            await repo.UpdateAsync(loaded);
            await uow.CommitAsync();

            var again = await repo.FindByIdAsync(t.Id);
            again!.IsDone.Should().BeTrue();

            await repo.DeleteAsync(again);
            await uow.CommitAsync();

            var missing = await repo.FindByIdAsync(t.Id);
            missing.Should().BeNull();
        }
        finally
        {
            db.Dispose();
        }
    }
}
