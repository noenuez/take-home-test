using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Commands.CreateLoan;

public sealed record CreateLoanCommand(string ApplicantName, decimal Amount)
    : ICommand<LoanResponse>;
