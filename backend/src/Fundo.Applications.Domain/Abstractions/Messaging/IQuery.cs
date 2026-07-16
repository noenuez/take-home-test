using MediatR;

namespace Fundo.Applications.Domain.Abstractions.Messaging;

/// <summary>
/// Marks a read-only request that returns a payload.
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>;
