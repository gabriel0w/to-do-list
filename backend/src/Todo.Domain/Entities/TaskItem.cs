using Todo.Domain.Common;

namespace Todo.Domain.Entities;

public class TaskItem : AbstractEntity<int>
{
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
