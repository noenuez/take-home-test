namespace Fundo.Applications.Data.Repositories;

public interface IWritableRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}
