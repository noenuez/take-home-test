using Fundo.Applications.Data;
using Fundo.Applications.Data.Models;
using Fundo.Applications.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Services.Tests.Integration;

/// <summary>
/// Boots the real WebApi pipeline (controllers, MediatR, validation and exception
/// handlers) but swaps PostgreSQL for an isolated in-memory database and skips the
/// FluentMigrator run via the "Testing" environment.
/// </summary>
public sealed class LoanApiFactory : WebApplicationFactory<Startup>
{
    private readonly string _databaseName = $"loans-tests-{Guid.NewGuid()}";

    static LoanApiFactory()
    {
        // AddPersistence resolves the connection string at startup; the value is
        // never used because the DbContext is replaced below.
        Environment.SetEnvironmentVariable(
            "DB_CONNECTION",
            "Host=localhost;Port=5432;Database=fundo_tests;Username=test;Password=test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Strip every EF registration tied to the Npgsql provider. EF Core 10
            // applies all IDbContextOptionsConfiguration<FundoDbContext> delegates,
            // so leaving the Npgsql one in place registers two providers at once.
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<FundoDbContext>) ||
                    d.ServiceType == typeof(FundoDbContext) ||
                    (d.ServiceType.FullName?.Contains("IDbContextOptionsConfiguration", StringComparison.Ordinal) == true
                        && d.ServiceType.GenericTypeArguments.Contains(typeof(FundoDbContext))))
                .ToList();

            foreach (var descriptor in toRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<FundoDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    public async Task ResetAndSeedAsync(params Loan[] loans)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FundoDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (loans.Length > 0)
        {
            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();
        }
    }
}
