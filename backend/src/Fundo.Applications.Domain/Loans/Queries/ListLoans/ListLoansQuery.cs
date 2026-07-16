using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Queries.ListLoans;

public sealed record ListLoansQuery : IQuery<IReadOnlyList<LoanResponse>>;
