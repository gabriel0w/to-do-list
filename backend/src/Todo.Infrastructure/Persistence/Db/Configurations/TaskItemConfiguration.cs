using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Db.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> task)
    {
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

