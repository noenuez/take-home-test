using Ardalis.Specification;

namespace Fundo.Applications.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> AsQueryable(bool tracking = false);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListPagedAsync(PageRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListPagedAsync(
        PageRequest request,
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);
    Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
