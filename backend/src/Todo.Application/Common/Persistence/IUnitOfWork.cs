using Microsoft.EntityFrameworkCore;

namespace Todo.Application.Common.Persistence;

public interface IUnitOfWork<TDb> where TDb : DbContext
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

