using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Db;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var task = modelBuilder.Entity<TaskItem>();
        task.ToTable("tasks");
        task.HasKey(t => t.Id);
        task.Property(t => t.Id).ValueGeneratedOnAdd();
        task.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        task.Property(t => t.IsDone)
            .HasDefaultValue(false);
        task.Property(t => t.OrderIndex)
            .HasDefaultValue(0);
        task.Property(t => t.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        task.HasIndex(t => new { t.IsDone, t.OrderIndex, t.CreatedAt });
    }
}
