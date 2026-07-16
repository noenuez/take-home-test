using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Loans.Commands.CreateLoan;
using Fundo.Applications.Domain.Loans.Commands.RegisterPayment;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Loans;

public class LoanCommandValidatorsTests
{
    private readonly CreateLoanCommandValidator _createValidator = new();
    private readonly Mock<IRepository<Loan>> _loans = new();

    private RegisterPaymentCommandValidator PaymentValidator() => new(_loans.Object);

    private void SetupLoan(Loan loan) =>
        _loans
            .Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

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
    public async Task RegisterPayment_NonPositiveAmount_ShouldFail(decimal amount)
    {
        var result = await PaymentValidator()
            .TestValidateAsync(new RegisterPaymentCommand(Guid.NewGuid(), amount));
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public async Task RegisterPayment_EmptyLoanId_ShouldFail()
    {
        var result = await PaymentValidator()
            .TestValidateAsync(new RegisterPaymentCommand(Guid.Empty, 100m));
        result.ShouldHaveValidationErrorFor(x => x.LoanId);
    }

    [Fact]
    public async Task RegisterPayment_AboveBalance_ShouldFail()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 500m, Status = LoanStatus.Active };
        SetupLoan(loan);

        var result = await PaymentValidator()
            .TestValidateAsync(new RegisterPaymentCommand(loan.Id, 600m));

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public async Task RegisterPayment_AlreadyPaidLoan_ShouldFail()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 0m, Status = LoanStatus.Paid };
        SetupLoan(loan);

        var result = await PaymentValidator()
            .TestValidateAsync(new RegisterPaymentCommand(loan.Id, 100m));

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public async Task RegisterPayment_ValidPayment_ShouldPass()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 500m, Status = LoanStatus.Active };
        SetupLoan(loan);

        var result = await PaymentValidator()
            .TestValidateAsync(new RegisterPaymentCommand(loan.Id, 200m));

        result.ShouldNotHaveAnyValidationErrors();
    }
}
