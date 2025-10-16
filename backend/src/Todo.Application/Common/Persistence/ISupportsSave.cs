namespace Todo.Application.Common.Persistence;

public interface ISupportsSave<TEntity, TId>
{
    TEntity Save(TEntity entity);
    Task<TEntity> SaveAsync(TEntity entity);
    IEnumerable<TEntity> Save(IEnumerable<TEntity> entities);
    TEntity SaveOrUpdate(TEntity entity);
    IEnumerable<TEntity> SaveOrUpdate(IEnumerable<TEntity> entities);
    TEntity Update(TEntity entity);
    IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);
}

