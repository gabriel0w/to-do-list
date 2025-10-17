using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Persistence.Db;
using Todo.Domain.Entities;

namespace Todo.Api.Dev;

public static class DevDataSeeder
{
    public static async Task SeedAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            await db.Database.MigrateAsync();

            if (!await db.Tasks.AnyAsync())
            {
                var now = DateTime.UtcNow;
                db.Tasks.AddRange(
                    new TaskItem { Title = "Buy milk", OrderIndex = 1, CreatedAt = now.AddMinutes(-5) },
                    new TaskItem { Title = "Read a book", OrderIndex = 2, CreatedAt = now.AddMinutes(-4) },
                    new TaskItem { Title = "Write code", OrderIndex = 3, CreatedAt = now.AddMinutes(-3) }
                );
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Do not crash the app if database is unavailable in development.
            app.Logger.LogWarning(ex, "Dev seed skipped: database unavailable or migration failed.");
        }
    }
}
