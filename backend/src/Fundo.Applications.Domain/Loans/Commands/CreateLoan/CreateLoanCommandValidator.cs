using FluentValidation;

namespace Fundo.Applications.Domain.Loans.Commands.CreateLoan;

public sealed class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
{
    public CreateLoanCommandValidator()
    {
        RuleFor(x => x.ApplicantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
