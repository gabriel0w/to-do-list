namespace Todo.Application.Features.Tasks.Contracts;

public class ReorderItem
{
    public int Id { get; set; }
    public int OrderIndex { get; set; }
}

public class ReorderTasksRequest
{
    public List<ReorderItem> Items { get; set; } = new();
}

