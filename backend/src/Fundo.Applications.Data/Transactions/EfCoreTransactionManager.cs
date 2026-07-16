using Microsoft.EntityFrameworkCore.Storage;

namespace Fundo.Applications.Data.Transactions;

public sealed class EfCoreTransactionManager : ITransactionManager
{
    private readonly FundoDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    public EfCoreTransactionManager(FundoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction ??= await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}
