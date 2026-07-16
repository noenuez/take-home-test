using MediatR;

namespace Fundo.Applications.Domain.Abstractions.Messaging;

/// <summary>
/// Marks a request that changes state and returns no payload.
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Marks a request that changes state and returns a payload.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>;
