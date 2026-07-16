using Fundo.Applications.Domain.Loans.Commands.CreateLoan;
using Fundo.Applications.Domain.Loans.Commands.RegisterPayment;
using FluentValidation.TestHelper;
using Xunit;

namespace Fundo.Services.Tests.Unit.Loans;

public class LoanCommandValidatorsTests
{
    private readonly CreateLoanCommandValidator _createValidator = new();
    private readonly RegisterPaymentCommandValidator _paymentValidator = new();

    [Theory]
    [InlineData("", 1000)]
    [InlineData("Maria", 0)]
    [InlineData("Maria", -5)]
    public void CreateLoan_InvalidInput_ShouldFail(string applicantName, decimal amount)
    {
        var result = _createValidator.TestValidate(new CreateLoanCommand(applicantName, amount));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateLoan_ValidInput_ShouldPass()
    {
        var result = _createValidator.TestValidate(new CreateLoanCommand("Maria Silva", 1500m));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RegisterPayment_NonPositiveAmount_ShouldFail(decimal amount)
    {
        var result = _paymentValidator.TestValidate(new RegisterPaymentCommand(Guid.NewGuid(), amount));
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void RegisterPayment_EmptyLoanId_ShouldFail()
    {
        var result = _paymentValidator.TestValidate(new RegisterPaymentCommand(Guid.Empty, 100m));
        result.ShouldHaveValidationErrorFor(x => x.LoanId);
    }
}
