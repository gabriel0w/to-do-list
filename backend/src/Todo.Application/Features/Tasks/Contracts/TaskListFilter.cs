namespace Todo.Application.Features.Tasks.Contracts;

public enum TaskStatusFilter { All, Open, Done }
public enum TaskSortField { CreatedAt, OrderIndex }
public enum SortDirection { Asc, Desc }

public class TaskListFilter
{
    public TaskStatusFilter Status { get; set; } = TaskStatusFilter.All;
    public TaskSortField Sort { get; set; } = TaskSortField.OrderIndex;
    public SortDirection Direction { get; set; } = SortDirection.Asc;
}
