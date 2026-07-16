using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Queries.GetLoanById;

public sealed record GetLoanByIdQuery(Guid Id) : IQuery<LoanResponse>;
