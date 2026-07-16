using Fundo.Applications.Data.Auditing;
using Fundo.Applications.Data.Migrations;
using Fundo.Applications.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Data;

public class FundoDbContext : DbContext, IUnitOfWork
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public FundoDbContext(
        DbContextOptions<FundoDbContext> options,
        ICurrentUserAccessor currentUserAccessor) : base(options)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(DatabaseSchema.Fundo);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FundoDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var entries = ChangeTracker.Entries<IAuditable>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        var now = DateTime.UtcNow;
        var username = _currentUserAccessor.GetCurrentUsername();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = username;

                if (entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = now;
                }

                entry.Entity.UpdatedBy = null;
                entry.Entity.UpdatedAt = null;
                continue;
            }

            entry.Entity.UpdatedBy = username;
            entry.Entity.UpdatedAt = now;
        }
    }
}
