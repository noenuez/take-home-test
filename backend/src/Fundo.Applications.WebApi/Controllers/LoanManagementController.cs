using Fundo.Applications.Domain.Loans.Commands.CreateLoan;
using Fundo.Applications.Domain.Loans.Commands.RegisterPayment;
using Fundo.Applications.Domain.Loans.Contracts;
using Fundo.Applications.Domain.Loans.Queries.GetLoanById;
using Fundo.Applications.Domain.Loans.Queries.ListLoans;
using Fundo.Applications.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers;

[ApiController]
[Route("loans")]
public class LoanManagementController : ControllerBase
{
    private readonly ISender _sender;

    public LoanManagementController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Creates a new loan. The current balance starts equal to the requested amount.</summary>
    [HttpPost]
    public async Task<ActionResult<LoanResponse>> Create(
        [FromBody] CreateLoanCommand command,
        CancellationToken cancellationToken)
    {
        var loan = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    /// <summary>Retrieves the details of a single loan.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetLoanByIdQuery(id), cancellationToken));

    /// <summary>Lists all loans, newest first.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LoanResponse>>> List(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new ListLoansQuery(), cancellationToken));

    /// <summary>Registers a payment against a loan, deducting it from the current balance.</summary>
    [HttpPost("{id:guid}/payment")]
    public async Task<ActionResult<LoanResponse>> RegisterPayment(
        Guid id,
        [FromBody] PaymentRequest request,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(new RegisterPaymentCommand(id, request.Amount), cancellationToken));
}
