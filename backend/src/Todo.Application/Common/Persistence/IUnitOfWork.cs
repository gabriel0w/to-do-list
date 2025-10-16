namespace Todo.Application.Common.Persistence;

public interface IUnitOfWork
{
    bool IsTransactionOpen { get; }
    void BeginTransaction(System.Data.IsolationLevel isolationLevel);
    Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel);
    void Commit();
    Task CommitAsync();
    void Rollback();
    Task RollbackAsync();
    Task SaveChangesAsync();
}
