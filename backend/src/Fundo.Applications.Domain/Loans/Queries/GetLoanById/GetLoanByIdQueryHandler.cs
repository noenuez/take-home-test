using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Exceptions;
using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Queries.GetLoanById;

public sealed class GetLoanByIdQueryHandler
    : IQueryHandler<GetLoanByIdQuery, LoanResponse>
{
    private readonly IRepository<Loan> _loans;

    public GetLoanByIdQueryHandler(IRepository<Loan> loans)
    {
        _loans = loans;
    }

    public async Task<LoanResponse> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        var loan = await _loans.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For(nameof(Loan), request.Id);

        return loan.ToResponse();
    }
}
