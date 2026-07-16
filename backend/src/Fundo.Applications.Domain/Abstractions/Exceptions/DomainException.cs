namespace Fundo.Applications.Domain.Abstractions.Exceptions;

/// <summary>
/// Thrown when a business rule is violated on a valid, existing aggregate
/// (e.g. paying a loan that is already settled). Maps to HTTP 409.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
