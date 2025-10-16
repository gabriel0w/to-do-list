using System.Linq.Expressions;
using Todo.Domain.Common;

namespace Todo.Application.Common.Persistence;

public interface IRepository<TEntity, TId> : ISupportsSave<TEntity, TId>, ISupportsDelete<TEntity, TId>
    where TEntity : AbstractEntity<TId>
{
    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    TEntity? FindById(TId id);
    Task<TEntity?> FindByIdAsync(TId id);
}

