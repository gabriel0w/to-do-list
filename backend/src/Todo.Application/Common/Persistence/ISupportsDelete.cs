namespace Todo.Application.Common.Persistence;

public interface ISupportsDelete<TEntity, TId>
{
    void Delete(TEntity entity);
    void Delete(IEnumerable<TEntity> entities);
    void DeleteById(TId id);
}

