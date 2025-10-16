namespace Todo.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    bool IsTransactionOpen { get; }
    bool HasChanges();
    void SetReadOnly();
    void BeginTransaction(System.Data.IsolationLevel isolationLevel);
    Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel);
    void Commit();
    Task CommitAsync();
    void Rollback();
    Task RollbackAsync();
    Task SaveChangesAsync();
}
