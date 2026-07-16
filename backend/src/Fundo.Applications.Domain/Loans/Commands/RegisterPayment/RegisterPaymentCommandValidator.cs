using FluentValidation;

namespace Fundo.Applications.Domain.Loans.Commands.RegisterPayment;

public sealed class RegisterPaymentCommandValidator : AbstractValidator<RegisterPaymentCommand>
{
    public RegisterPaymentCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
