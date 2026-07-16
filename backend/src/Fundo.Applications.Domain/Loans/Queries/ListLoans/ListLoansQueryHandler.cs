using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Domain.Loans.Queries.ListLoans;

public sealed class ListLoansQueryHandler
    : IQueryHandler<ListLoansQuery, IReadOnlyList<LoanResponse>>
{
    private readonly IRepository<Loan> _loans;

    public ListLoansQueryHandler(IRepository<Loan> loans)
    {
        _loans = loans;
    }

    public async Task<IReadOnlyList<LoanResponse>> Handle(ListLoansQuery request, CancellationToken cancellationToken)
    {
        var loans = await _loans.AsQueryable()
            .OrderByDescending(loan => loan.CreatedAt)
            .ToListAsync(cancellationToken);

        return loans.Select(loan => loan.ToResponse()).ToList();
    }
}
