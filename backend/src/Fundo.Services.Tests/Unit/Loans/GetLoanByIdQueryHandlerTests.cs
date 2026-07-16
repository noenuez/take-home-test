using Fundo.Applications.Data.Models;
using Fundo.Applications.Data.Repositories;
using Fundo.Applications.Domain.Abstractions.Exceptions;
using Fundo.Applications.Domain.Loans.Queries.GetLoanById;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Loans;

public class GetLoanByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Loan>> _repository = new();

    [Fact]
    public async Task Handle_ExistingLoan_ShouldReturnResponse()
    {
        var loan = new Loan { ApplicantName = "Ana", Amount = 800m, CurrentBalance = 800m };
        _repository
            .Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        var response = await new GetLoanByIdQueryHandler(_repository.Object)
            .Handle(new GetLoanByIdQuery(loan.Id), default);

        response.Id.Should().Be(loan.Id);
        response.ApplicantName.Should().Be("Ana");
    }

    [Fact]
    public async Task Handle_MissingLoan_ShouldThrowNotFoundException()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var act = () => new GetLoanByIdQueryHandler(_repository.Object)
            .Handle(new GetLoanByIdQuery(Guid.NewGuid()), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
