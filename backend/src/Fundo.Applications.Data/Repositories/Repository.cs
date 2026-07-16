using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Data.Repositories;

public class Repository<TEntity> : IWritableRepository<TEntity> where TEntity : class
{
    protected readonly FundoDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(FundoDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> AsQueryable(bool tracking = false)
    {
        return tracking ? DbSet.AsQueryable() : DbSet.AsNoTracking();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([id], cancellationToken);

    public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        => AsQueryable().ToListAsync(cancellationToken);

    public Task<PagedResult<TEntity>> ListPagedAsync(PageRequest request, CancellationToken cancellationToken = default)
        => ListPagedAsync(request, new PagedDefaultSpecification(), cancellationToken);

    public async Task<PagedResult<TEntity>> ListPagedAsync(
        PageRequest request,
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = ApplySpecification(specification);
        var pageNumber = request.NormalizedPageNumber;
        var pageSize = request.NormalizedPageSize;
        var skip = (pageNumber - 1) * pageSize;

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var items = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).ToListAsync(cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).AnyAsync(cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => DbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Delete(TEntity entity) => DbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        => SpecificationEvaluator.Default.GetQuery(AsQueryable(), specification);

    private sealed class PagedDefaultSpecification : Specification<TEntity>
    {
    }
}
