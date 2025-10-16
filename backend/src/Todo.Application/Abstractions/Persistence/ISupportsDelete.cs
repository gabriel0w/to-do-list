namespace Todo.Application.Abstractions.Persistence;

public interface ISupportsDelete<TEntity, TId>
{
    void Delete(TEntity entity);
    void Delete(IEnumerable<TEntity> entities);
    void DeleteById(TId id);
}
