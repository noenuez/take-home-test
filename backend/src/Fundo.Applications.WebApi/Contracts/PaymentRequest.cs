namespace Fundo.Applications.WebApi.Contracts;

/// <summary>Body for <c>POST /loans/{id}/payment</c>.</summary>
public sealed record PaymentRequest(decimal Amount);
