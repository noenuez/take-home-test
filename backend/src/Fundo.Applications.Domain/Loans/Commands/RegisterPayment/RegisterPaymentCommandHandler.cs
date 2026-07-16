using Fundo.Applications.Data;
using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Exceptions;
using Fundo.Applications.Domain.Abstractions.Messaging;
using Fundo.Applications.Domain.Loans.Contracts;

namespace Fundo.Applications.Domain.Loans.Commands.RegisterPayment;

public sealed class RegisterPaymentCommandHandler
    : ICommandHandler<RegisterPaymentCommand, LoanResponse>
{
    private readonly IWritableRepository<Loan> _loans;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterPaymentCommandHandler(IWritableRepository<Loan> loans, IUnitOfWork unitOfWork)
    {
        _loans = loans;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoanResponse> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken)
    {
        // Business rules (already paid / payment exceeds balance) are enforced by
        // RegisterPaymentCommandValidator in the pipeline before this handler runs.
        var loan = await _loans.GetByIdAsync(request.LoanId, cancellationToken)
            ?? throw NotFoundException.For(nameof(Loan), request.LoanId);

        loan.ApplyPayment(request.Amount);

        _loans.Update(loan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return loan.ToResponse();
    }
}
