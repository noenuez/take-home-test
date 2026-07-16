using Fundo.Applications.Data;
using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Loans.Commands.CreateLoan;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Loans;

public class CreateLoanCommandHandlerTests
{
    private readonly Mock<IWritableRepository<Loan>> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldStartBalanceEqualToAmount_AndPersist()
    {
        Loan? persisted = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
            .Callback<Loan, CancellationToken>((loan, _) => persisted = loan)
            .Returns(Task.CompletedTask);
        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateLoanCommandHandler(_repository.Object, _unitOfWork.Object);

        var response = await handler.Handle(new CreateLoanCommand("  Maria Silva  ", 1500m), default);

        response.ApplicantName.Should().Be("Maria Silva");
        response.Amount.Should().Be(1500m);
        response.CurrentBalance.Should().Be(1500m);
        response.Status.Should().Be("Active");

        persisted.Should().NotBeNull();
        persisted!.CurrentBalance.Should().Be(1500m);
        persisted.Status.Should().Be(LoanStatus.Active);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
