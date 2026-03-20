using BolluFlow.Application.Features.Finance.CreateTransaction;
using BolluFlow.Application.Features.Finance.DeleteTransaction;
using BolluFlow.Application.Features.Finance.ListTransactions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Finance;

[ApiController]
[Route("api/finance/transactions")]
[Authorize]
public class FinanceTransactionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FinancialTransactionDto>>> List([FromQuery] string month, CancellationToken ct)
        => (await mediator.Send(new ListFinancialTransactionsQuery(month), ct)).ToActionResult();

    [HttpPost]
    public async Task<ActionResult<FinancialTransactionDto>> Create([FromBody] CreateFinancialTransactionCommand command, CancellationToken ct)
        => (await mediator.Send(command, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<object>> Delete([FromRoute] Guid id, CancellationToken ct)
        => (await mediator.Send(new DeleteFinancialTransactionCommand(id), ct)).ToActionResult();
}
