using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using FluentValidation;

namespace Fundo.Applications.Domain.Loans.Commands.RegisterPayment;

public sealed class RegisterPaymentCommandValidator : AbstractValidator<RegisterPaymentCommand>
{
    private readonly IRepository<Loan> _loans;

    public RegisterPaymentCommandValidator(IRepository<Loan> loans)
    {
        _loans = loans;

        RuleFor(x => x.LoanId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        // State-dependent rules: only run when the basic input is valid, so we don't
        // hit the database for an obviously bad request. A missing loan is left to the
        // handler (404); here we only guard the business rules on an existing loan.
        RuleFor(x => x)
            .CustomAsync(ValidateAgainstLoanState)
            .When(x => x.LoanId != Guid.Empty && x.Amount > 0);
    }

    private async Task ValidateAgainstLoanState(
        RegisterPaymentCommand command,
        ValidationContext<RegisterPaymentCommand> context,
        CancellationToken cancellationToken)
    {
        var loan = await _loans.GetByIdAsync(command.LoanId, cancellationToken);
        if (loan is null)
        {
            return;
        }

        if (loan.Status == LoanStatus.Paid)
        {
            context.AddFailure(nameof(command.Amount), "The loan is already fully paid.");
            return;
        }

        if (command.Amount > loan.CurrentBalance)
        {
            context.AddFailure(
                nameof(command.Amount),
                $"Payment of {command.Amount:0.00} exceeds the current balance of {loan.CurrentBalance:0.00}.");
        }
    }
}
