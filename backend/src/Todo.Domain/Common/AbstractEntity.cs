namespace Todo.Domain.Common;

public abstract class AbstractEntity<TId>
{
    public TId Id { get; set; } = default!;
}

