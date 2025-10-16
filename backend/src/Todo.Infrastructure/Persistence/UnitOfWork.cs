using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Todo.Application.Abstractions.Persistence;

namespace Todo.Infrastructure.Persistence;

public class UnitOfWork<TDb> : IUnitOfWork<TDb> where TDb : DbContext
{
    private readonly TDb _dbContext;
    private IDbContextTransaction? _transaction;
    private bool _readOnly;

    public UnitOfWork(TDb dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsTransactionOpen => _transaction is not null;

    public bool HasChanges() => _dbContext.ChangeTracker.HasChanges();

    public void SetReadOnly()
    {
        _readOnly = true;
        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
    {
        _transaction = _dbContext.Database.BeginTransaction(isolationLevel);
    }

    public async Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
    }

    public void Commit()
    {
        if (!_readOnly)
        {
            _dbContext.SaveChanges();
        }
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task CommitAsync()
    {
        if (!_readOnly)
        {
            await _dbContext.SaveChangesAsync();
        }
        if (_transaction is not null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
