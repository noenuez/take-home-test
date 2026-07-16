namespace Fundo.Applications.Domain.Abstractions.Exceptions;

/// <summary>
/// Thrown when a requested aggregate does not exist. Maps to HTTP 404.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, object key)
        => new($"{entity} with id '{key}' was not found.");
}
