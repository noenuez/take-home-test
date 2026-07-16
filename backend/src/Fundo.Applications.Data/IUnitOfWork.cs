namespace Fundo.Applications.Data;

/// <summary>
/// Abstraction over the EF Core change-tracker commit, so command handlers can
/// persist changes without depending directly on <see cref="FundoDbContext"/>.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
