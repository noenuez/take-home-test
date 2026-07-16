using Fundo.Applications.Data;
using Fundo.Applications.Data.Auditing;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Data.Transactions;
using Fundo.Applications.Domain;
using Fundo.Applications.WebApi.Exceptions;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.WebApi.Configurations;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        var connectionString = ResolveConnectionString();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        services.AddDbContext<FundoDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(FundoDbContext).Assembly).For.Migrations());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IWritableRepository<>), typeof(Repository<>));
        services.AddScoped<ITransactionManager, EfCoreTransactionManager>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FundoDbContext>());

        return services;
    }

    public const string FrontendCorsPolicy = "frontend";

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDomain();

        // Order matters: the first handler that recognizes the exception wins.
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails();

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy => policy
                .WithOrigins(frontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });

        return services;
    }

    public static void RunDatabaseMigrations(this IApplicationBuilder app)
    {
        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigrations");
        var runner = services.GetRequiredService<IMigrationRunner>();

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                runner.MigrateUp();
                logger.LogInformation("Database migrations applied successfully.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Migration attempt {Attempt}/{MaxAttempts} failed (database may still be starting); retrying in {Delay}s.",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                Thread.Sleep(delay);
            }
        }
    }

    private static string ResolveConnectionString()
        => Environment.GetEnvironmentVariable("DB_CONNECTION")
            ?? throw new InvalidOperationException("DB_CONNECTION is not configured.");
}
