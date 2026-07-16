using Fundo.Applications.Domain.Abstractions.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Exceptions;

/// <summary>Maps <see cref="NotFoundException"/> to an RFC 9110 problem+json 404 response.</summary>
public sealed class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found",
            Title = "Resource not found.",
            Status = StatusCodes.Status404NotFound,
            Detail = notFoundException.Message,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
