using Fundo.Applications.Domain.Abstractions.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Exceptions;

/// <summary>Maps <see cref="DomainException"/> (business-rule violations) to a 409 response.</summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-409-conflict",
            Title = "Business rule violation.",
            Status = StatusCodes.Status409Conflict,
            Detail = domainException.Message,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
