using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Commands.RegisterPayment;

public sealed record RegisterPaymentCommand(Guid LoanId, decimal Amount)
    : ICommand<LoanResponse>;
