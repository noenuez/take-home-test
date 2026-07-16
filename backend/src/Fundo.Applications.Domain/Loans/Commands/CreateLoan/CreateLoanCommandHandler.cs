using Fundo.Applications.Data;
using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Commands.CreateLoan;

public sealed class CreateLoanCommandHandler
    : ICommandHandler<CreateLoanCommand, LoanResponse>
{
    private readonly IWritableRepository<Loan> _loans;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLoanCommandHandler(IWritableRepository<Loan> loans, IUnitOfWork unitOfWork)
    {
        _loans = loans;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoanResponse> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = new Loan
        {
            ApplicantName = request.ApplicantName.Trim(),
            Amount = request.Amount,
            CurrentBalance = request.Amount,
            Status = LoanStatus.Active
        };

        await _loans.AddAsync(loan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return loan.ToResponse();
    }
}
