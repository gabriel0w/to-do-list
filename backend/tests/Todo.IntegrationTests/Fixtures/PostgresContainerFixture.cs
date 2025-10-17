using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Persistence.Db;
using Xunit;

namespace Todo.IntegrationTests.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    public string ConnectionString => _container.GetConnectionString();

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("tododb")
        .WithUsername("todo")
        .WithPassword("todo")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        using var db = new TodoDbContext(options);
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}
