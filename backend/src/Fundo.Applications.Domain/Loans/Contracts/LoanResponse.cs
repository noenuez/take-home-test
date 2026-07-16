using Fundo.Applications.Data.Models;

namespace Fundo.Applications.Domain.Loans.Contracts;

public sealed record LoanResponse(
    Guid Id,
    string ApplicantName,
    decimal Amount,
    decimal CurrentBalance,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public static class LoanMapping
{
    public static LoanResponse ToResponse(this Loan loan)
        => new(
            loan.Id,
            loan.ApplicantName,
            loan.Amount,
            loan.CurrentBalance,
            loan.Status.ToString(),
            loan.CreatedAt,
            loan.UpdatedAt);
}
