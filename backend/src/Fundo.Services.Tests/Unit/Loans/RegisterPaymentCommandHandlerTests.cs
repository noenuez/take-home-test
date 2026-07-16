using Fundo.Applications.Data;
using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Exceptions;
using Fundo.Applications.Domain.Loans.Commands.RegisterPayment;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Loans;

public class RegisterPaymentCommandHandlerTests
{
    private readonly Mock<IWritableRepository<Loan>> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RegisterPaymentCommandHandler CreateHandler() =>
        new(_repository.Object, _unitOfWork.Object);

    private void SetupLoan(Loan loan) =>
        _repository
            .Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

    [Fact]
    public async Task Handle_PartialPayment_ShouldReduceBalanceAndStayActive()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 500m, Status = LoanStatus.Active };
        SetupLoan(loan);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var response = await CreateHandler().Handle(new RegisterPaymentCommand(loan.Id, 200m), default);

        response.CurrentBalance.Should().Be(300m);
        response.Status.Should().Be("Active");
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FullPayment_ShouldZeroBalanceAndMarkPaid()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 500m, Status = LoanStatus.Active };
        SetupLoan(loan);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var response = await CreateHandler().Handle(new RegisterPaymentCommand(loan.Id, 500m), default);

        response.CurrentBalance.Should().Be(0m);
        response.Status.Should().Be("Paid");
    }

    [Fact]
    public async Task Handle_PaymentAboveBalance_ShouldThrowDomainException()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 500m, Status = LoanStatus.Active };
        SetupLoan(loan);

        var act = () => CreateHandler().Handle(new RegisterPaymentCommand(loan.Id, 600m), default);

        await act.Should().ThrowAsync<DomainException>();
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyPaidLoan_ShouldThrowDomainException()
    {
        var loan = new Loan { Amount = 1000m, CurrentBalance = 0m, Status = LoanStatus.Paid };
        SetupLoan(loan);

        var act = () => CreateHandler().Handle(new RegisterPaymentCommand(loan.Id, 100m), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_MissingLoan_ShouldThrowNotFoundException()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var act = () => CreateHandler().Handle(new RegisterPaymentCommand(Guid.NewGuid(), 100m), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
