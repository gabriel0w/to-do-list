using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Persistence.Db;
using Todo.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Todo.Api.Dev;

public static class DevDataSeeder
{
    public static async Task SeedAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var reset = cfg.GetValue<bool>("Seed:Reset", false);
            var force = cfg.GetValue<bool>("Seed:Force", false);
            await db.Database.MigrateAsync();

            if (reset)
            {
                // Clear existing tasks to ensure deterministic dev data
                try
                {
#if NET8_0_OR_GREATER
                    await db.Tasks.ExecuteDeleteAsync();
#else
                    db.Tasks.RemoveRange(db.Tasks);
                    await db.SaveChangesAsync();
#endif
                }
                catch
                {
                    // Fallback if provider doesn't support ExecuteDeleteAsync
                    var all = await db.Tasks.ToListAsync();
                    db.Tasks.RemoveRange(all);
                    await db.SaveChangesAsync();
                }
            }

            if (reset || force || !await db.Tasks.AnyAsync())
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
