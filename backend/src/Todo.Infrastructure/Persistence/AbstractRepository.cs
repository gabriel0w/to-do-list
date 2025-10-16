using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.Persistence;
using Todo.Domain.Common;

namespace Todo.Infrastructure.Persistence;

public abstract class AbstractRepository<TDb, TEntity, TId> : IRepository<TEntity, TId>
    where TDb : DbContext
    where TEntity : AbstractEntity<TId>
{
    protected readonly TDb _dbContext;
    protected readonly IUnitOfWork<TDb> _unitOfWork;
    protected readonly DbSet<TEntity> DbSet;

    public bool IsTransactionOpen => _unitOfWork.IsTransactionOpen;

    protected AbstractRepository(TDb dbContext, IUnitOfWork<TDb> unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        DbSet = dbContext.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        => DbSet.Where(predicate);

    public virtual TEntity? FindById(TId id)
        => DbSet.Find(id);

    public virtual async Task<TEntity?> FindByIdAsync(TId id)
        => await DbSet.FindAsync(id);

    public virtual IQueryable<TEntity> GetAll()
        => DbSet.AsQueryable();

    public virtual TEntity Save(TEntity entity)
    {
        DbSet.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public virtual async Task<TEntity> SaveAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual IEnumerable<TEntity> Save(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
        _dbContext.SaveChanges();
        return entities;
    }

    public virtual TEntity SaveOrUpdate(TEntity entity)
        => Update(entity);

    public virtual IEnumerable<TEntity> SaveOrUpdate(IEnumerable<TEntity> entities)
        => Update(entities);

    public virtual TEntity Update(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
            _dbContext.Attach(entity);

        DbSet.Update(entity);
        return entity;
    }

    public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
        return entities;
    }

    public virtual void Delete(TEntity entity)
        => DbSet.Remove(entity);

    public virtual void Delete(IEnumerable<TEntity> entities)
        => DbSet.RemoveRange(entities);

    public virtual void DeleteById(TId id)
    {
        var entity = FindById(id);
        if (entity is not null) Delete(entity);
    }

    public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        => _unitOfWork.BeginTransaction(isolationLevel);

    public void Commit() => _unitOfWork.Commit();

    public void Rollback() => _unitOfWork.Rollback();

    public Task CommitAsync() => _unitOfWork.CommitAsync();

    public Task RollbackAsync() => _unitOfWork.RollbackAsync();

    public Task SaveChangesAsync() => _unitOfWork.SaveChangesAsync();
}

