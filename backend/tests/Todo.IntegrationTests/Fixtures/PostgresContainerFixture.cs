using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Persistence.Db;

namespace Todo.IntegrationTests.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    public string ConnectionString => $"Host=localhost;Port={_container.GetMappedPublicPort(5432)};Database=tododb;Username=todo;Password=todo";

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("postgres:16")
        .WithEnvironment("POSTGRES_DB", "tododb")
        .WithEnvironment("POSTGRES_USER", "todo")
        .WithEnvironment("POSTGRES_PASSWORD", "todo")
        .WithPortBinding(0, 5432)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
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

