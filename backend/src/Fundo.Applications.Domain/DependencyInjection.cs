using Fundo.Applications.Domain.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Applications.Domain;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the CQRS pipeline (MediatR handlers + validation behavior) and all
    /// FluentValidation validators discovered in the domain assembly.
    /// </summary>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}
