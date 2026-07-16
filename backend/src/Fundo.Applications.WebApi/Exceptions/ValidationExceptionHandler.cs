using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Exceptions;

/// <summary>
/// Translates FluentValidation failures thrown from the CQRS pipeline into an
/// RFC 9110 problem+json 400 response.
/// </summary>
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
            Title = "Request validation failed.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
